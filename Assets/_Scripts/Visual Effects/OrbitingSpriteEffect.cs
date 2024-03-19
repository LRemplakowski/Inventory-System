using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public class OrbitingSpriteEffect : SerializedMonoBehaviour, IVisualEffect, IOrbital
    {
        [ShowInInspector, ReadOnly]
        public string InstanceID => GetInstanceID().ToString();
        [field: SerializeField, ReadOnly]
        public string EffectID { get; private set; }
        [field: SerializeField]
        public float RotationOffset { get; private set; } = 15f;
        [field: SerializeField]
        public float RotationSpeed { get; private set; } = 90f;

        public GameObject GameObject => this.gameObject;
        public Transform OrbitalTransform => this.transform;
        public VisualEffectParent ParentingType => VisualEffectParent.Handler;

        public bool AllowOnlySingleInstance => false;

        [Title("Runtime")]
        [ShowInInspector]
        private VisualEffectContext _cachedContext;
        [ShowInInspector]
        private OrbitalEffectManager _orbitalManager;

        private float _selfDestructTime;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(EffectID))
                EffectID = Guid.NewGuid().ToString();
        }

        private void OnDestroy()
        {
            _orbitalManager.RemoveOrbital(this);
        }

        public void SelfGovernDuration()
        {
            _selfDestructTime = _cachedContext.Duration;
            _cachedContext.CoroutineRunner.StartCoroutine(SelfDestructAfterTime());
        }

        private IEnumerator SelfDestructAfterTime()
        {
            while (_selfDestructTime > 0)
            {
                _selfDestructTime -= Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        public void Initialize(VisualEffectContext context)
        {
            if (context.FollowTransform == null)
            {
                Debug.LogError("Orbiting Sprite effect requires a follow transform!");
                return;
            }
            _cachedContext = context;
            _orbitalManager = OrbitalEffectManager.GetManagerForTransfrom(context.FollowTransform);
            OrbitalTransform.SetParent(_orbitalManager.transform);
            OrbitalTransform.position = _orbitalManager.transform.position;
            _orbitalManager.AddOrbital(this);
            _orbitalManager.OrbitalSpinSpeed = RotationSpeed;
            if (context.SelfGovernDuration)
                SelfGovernDuration();
        }

        public void AddDuration(float duration)
        {
            _selfDestructTime += duration;
        }
    }
}
