using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace SunsetSystems.VisualEffects
{
    public class OrbitalEffectManager : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        private static readonly Dictionary<Transform, OrbitalEffectManager> _orbitalManagers = new();

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<string, List<IOrbital>> _orbitalEffects = new();

        private Transform _cachedOrbitalFollowTransform;
        [NonSerialized]
        public float OrbitalSpinSpeed;

        private void Update()
        {
            transform.Rotate(new(0, OrbitalSpinSpeed * Time.deltaTime, 0));
        }

        public void AddOrbital(IOrbital orbital)
        {
            if (_orbitalEffects.TryGetValue(orbital.EffectID, out List<IOrbital> orbitals))
            {
                orbitals.Add(orbital);
            }
            else
            {
                orbitals = new();
                orbitals.Add(orbital);
                _orbitalEffects[orbital.EffectID] = orbitals;
            }
            ArrangeOrbitals();
        }

        public void RemoveOrbital(IOrbital orbital)
        {
            if (_orbitalEffects.TryGetValue(orbital.EffectID, out List<IOrbital> orbitals))
            {
                orbitals.Remove(orbital);
                ArrangeOrbitals();
            }
        }

        private void ArrangeOrbitals()
        {
            foreach (string key in _orbitalEffects.Keys)
            {
                var orbitals = _orbitalEffects[key];
                for (int i = 0; i < orbitals.Count; i++)
                {
                    var orbital = orbitals[i];
                    orbital.OrbitalTransform.localRotation = Quaternion.Euler(0, (360f / orbitals.Count) * i, 0);
                }
            }
        }

        private void OnDestroy()
        {
            _orbitalManagers.Remove(_cachedOrbitalFollowTransform);
        }

        public static OrbitalEffectManager GetManagerForTransfrom(Transform transform)
        {
            if (transform == null)
                return null;
            if (_orbitalManagers.TryGetValue(transform, out OrbitalEffectManager result))
            {
                return result;
            }
            else
            {
                OrbitalEffectManager managerObject = new GameObject($"Orbiting Sprite Manager - {transform.name}").AddComponent<OrbitalEffectManager>();
                PositionConstraint constraint = managerObject.gameObject.AddComponent<PositionConstraint>();
                managerObject._cachedOrbitalFollowTransform = transform;
                managerObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
                constraint.AddSource(new() 
                { 
                    sourceTransform = transform,
                    weight = 1
                });
                constraint.translationOffset = Vector3.zero;
                constraint.locked = true;
                constraint.constraintActive = true;
                _orbitalManagers[transform] = managerObject;
                return managerObject;
            }
        }
    }
}
