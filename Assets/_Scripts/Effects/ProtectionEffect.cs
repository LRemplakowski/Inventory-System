using System;
using Sirenix.OdinInspector;
using SunsetSystems.VisualEffects;
using UnityEngine;

namespace SunsetSystems.Effects
{
    [CreateAssetMenu(fileName = "New Protection Effect", menuName = "Effects/Protection Effect")]
    public class ProtectionEffect : SerializedScriptableObject, IEffect
    {
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; } = Guid.NewGuid().ToString();
        [field: SerializeField]
        public float Duration { get; private set; }
        [field: SerializeField]
        public IVisualEffect VisualsPrefab { get; private set; }
        [field: SerializeField]
        public EffectTarget Target { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(ID))
                ID = Guid.NewGuid().ToString();
        }

        public void ApplyEffect(EffectContext context)
        {
            context.VisualEffectHandler.HandleVisualEffect(VisualsPrefab, context.VisualEffectSource, Duration);
        }

        public void CancelEffect(EffectContext context)
        {
            context.VisualEffectHandler.CancelEffect(context.VisualEffectSource, VisualsPrefab);
        }
    }
}
