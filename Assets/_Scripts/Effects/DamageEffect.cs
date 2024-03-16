using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Effects
{
    [CreateAssetMenu(fileName = "New Damage Effect", menuName = "Effects/Damage Effect")]
    public class DamageEffect : SerializedScriptableObject, IEffect
    {
        [field: SerializeField]
        public float Duration { get; private set; }
        [field: SerializeField]
        public GameObject VisualsPrefab { get; private set; }
        [field: SerializeField]
        public EffectTarget Target { get; private set; }

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
