using System;
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
        [SerializeField]
        private float _selfDuration = 0;
        [field: SerializeField]
        public float RotationOffset { get; private set; } = 15f;
        [field: SerializeField]
        public float RotationSpeed { get; private set; } = 90f;

        public GameObject GameObject => this.gameObject;
        public Transform OrbitalTransform => this.transform;
        public VisualEffectParent ParentingType => VisualEffectParent.Handler;

        private OrbitalEffectManager _orbitalManager;

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
            Destroy(gameObject, _selfDuration);
        }

        public void SetFollowTransform(Transform follow)
        {
            _orbitalManager = OrbitalEffectManager.GetManagerForTransfrom(follow);
            OrbitalTransform.SetParent(_orbitalManager.transform);
            OrbitalTransform.position = _orbitalManager.transform.position;
            _orbitalManager.AddOrbital(this);
            _orbitalManager.OrbitalSpinSpeed = RotationSpeed;
        }
    }
}
