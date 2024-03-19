using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace SunsetSystems.VisualEffects
{
    public class VisualEffectHandler : SerializedMonoBehaviour, IVisualEffectHandler
    {
        [SerializeField, Required]
        private Transform _effectFollowParent;
        [SerializeField, Required]
        private VolumeProfile _cameraVolumeProfile;
        [SerializeField, Required]
        private Transform _playerCameraRig;


        [ShowInInspector, ReadOnly]
        private Dictionary<string, VisualEffectData> _activeVisualEffects = new();

        private readonly List<string> _removeSourceIDs = new();

        private void Update()
        {
            foreach (string key in _activeVisualEffects.Keys)
            {
                var effectData = _activeVisualEffects[key];
                effectData.UpdateEffects();
                if (effectData.HasActiveInstances() is false)
                    _removeSourceIDs.Add(key);
            }
            _removeSourceIDs.ForEach(key => _activeVisualEffects.Remove(key));
            _removeSourceIDs.Clear();
        }

        public void HandleVisualEffect(IVisualEffect visualEffectPrefab, IVisualEffectSource source, float visualEffectDuration)
        {
            if (visualEffectPrefab == null)
            {
                Debug.LogError($"VFX Handler recieved a null VFX prefab! Source: {source}");
                return;
            }
            Transform effectParent = null;
            switch (visualEffectPrefab.ParentingType)
            {
                case VisualEffectParent.None:
                    break;
                case VisualEffectParent.Handler:
                    effectParent = _effectFollowParent;
                    break;
            }
            if (visualEffectPrefab.AllowOnlySingleInstance)
            {
                if (_activeVisualEffects.TryGetValue(source.ID, out VisualEffectData data))
                {
                    data.AddInstanceLifetime(visualEffectDuration);
                }
            }
            // For test task should work okay, in work enviroment should be replaced with object pooling.
            GameObject instanceGO = Instantiate(visualEffectPrefab.GameObject);
            if (instanceGO.TryGetComponent(out IVisualEffect effectInstance))
            {
                effectInstance.Initialize(new()
                {
                    FollowTransform = effectParent,
                    Duration = visualEffectDuration,
                    Position = _effectFollowParent.position,
                    VolumeProfile = _cameraVolumeProfile,
                    SelfGovernDuration = visualEffectDuration > 0,
                    PlayerCameraRig = _playerCameraRig,
                    CoroutineRunner = this
                });
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
                effectData.DestroyEffectInstance(effectInstance.InstanceID);
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
                    InstanceID = effectInstance.InstanceID,
                    EffectInstance = effectInstance.GameObject,
                    VisualEffectScript = effectInstance,
                    EffectStartTimestamp = time,
                    EffectDuration = duration
                };
                _effectInstances.Add(effectInstance.InstanceID, instanceData);
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

            public bool HasActiveInstances()
            {
                return _effectInstances.Count > 0;
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

            public void AddInstanceLifetime(float addedTime)
            {
                foreach (var instanceData in _effectInstances.Values)
                {
                    instanceData.EffectDuration += addedTime;
                    instanceData.VisualEffectScript.AddDuration(addedTime);
                }
            }
            
            public class VisualEffectInstanceData
            {
                public string InstanceID;
                public GameObject EffectInstance;
                public IVisualEffect VisualEffectScript;
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
