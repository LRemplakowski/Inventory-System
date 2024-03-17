using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public class VisualEffectHandler : SerializedMonoBehaviour, IVisualEffectHandler
    {
        [SerializeField, Required]
        private Transform _effectFollowParent;

        private Dictionary<string, VisualEffectData> _activeVisualEffects = new();

        public void HandleVisualEffect(IVisualEffect visualEffectPrefab, IVisualEffectSource source, float visualEffectDuration)
        {
            Transform effectParent = null;
            switch (visualEffectPrefab.ParentingType)
            {
                case VisualEffectParent.None:
                    break;
                case VisualEffectParent.Handler:
                    effectParent = _effectFollowParent;
                    break;
            }
            // For test task should work okay, in work enviroment should be replaced with object pooling.
            GameObject instanceGO = Instantiate(visualEffectPrefab.GameObject);
            if (instanceGO.TryGetComponent(out IVisualEffect effectInstance))
            {
                effectInstance.SetFollowTransform(effectParent);
                if (visualEffectDuration > 0)
                {
                    if (_activeVisualEffects.TryGetValue(source.ID, out VisualEffectData effectData))
                    {
                        effectData.AddEffectInstance(effectInstance, Time.time, visualEffectDuration);
                    }
                    else
                    {
                        effectData = new();
                        effectData.AddEffectInstance(effectInstance, Time.time, visualEffectDuration);
                        _activeVisualEffects[source.ID] = effectData;
                    }
                }
                else
                {
                    // Effects with duration of 0 are Instantiate-and-Forget.
                    effectInstance.SelfGovernDuration();
                }
            }
            else
            {
                // This should not happen but just in case.
                Destroy(instanceGO);
            }
        }

        public void CancelEffect(IVisualEffectSource source, IVisualEffect effectInstance)
        {
            if (_activeVisualEffects.TryGetValue(source.ID, out var effectData))
            {
                effectData.DestroyEffectInstance(effectInstance.ID);
            }
        }

        private class VisualEffectData
        {
            private Dictionary<string, VisualEffectInstanceData> _effectInstances = new();

            private readonly List<VisualEffectInstanceData> _instancesToRemove = new();

            public void AddEffectInstance(IVisualEffect effectInstance, float time, float duration)
            {
                var instanceData = new VisualEffectInstanceData()
                {
                    InstanceID = effectInstance.ID,
                    EffectInstance = effectInstance.GameObject,
                    EffectStartTimestamp = time,
                    EffectDuration = duration
                };
                _effectInstances.Add(effectInstance.ID, instanceData);
            }

            public void DestroyEffectInstance(string instanceID)
            {
                if (_effectInstances.TryGetValue(instanceID, out VisualEffectInstanceData instanceData))
                {
                    Destroy(instanceData.EffectInstance);
                    _effectInstances.Remove(instanceID);
                }
            }

            public void UpdateEffects()
            {
                foreach (var effectInstance in _effectInstances.Values)
                {
                    if (effectInstance.EvaluateEffectDurationEnd())
                        _instancesToRemove.Add(effectInstance);
                }
                DeleteDeadInstances();
            }

            private void DeleteDeadInstances()
            {
                foreach (var instance in _instancesToRemove)
                {
                    Destroy(instance.EffectInstance);
                    _effectInstances.Remove(instance.InstanceID);
                }
                _instancesToRemove.Clear();
            }
            
            public class VisualEffectInstanceData
            {
                public string InstanceID;
                public GameObject EffectInstance;
                public float EffectStartTimestamp;
                public float EffectDuration;

                public bool EvaluateEffectDurationEnd()
                {
                    return Time.time >= EffectStartTimestamp + EffectDuration;
                }
            }
        }
    }
}
