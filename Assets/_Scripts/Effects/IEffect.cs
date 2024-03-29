using SunsetSystems.VisualEffects;
using UnityEngine;

namespace SunsetSystems.Effects
{
    public interface IEffect : IVisualEffectSource
    {
        /// <summary>
        /// Duration of <= 0 means effect is instant.
        /// </summary>
        float Duration { get; }
        IVisualEffect VisualsPrefab { get; }
        EffectTarget Target { get; }

        void ApplyEffect(EffectContext context);

        void CancelEffect(EffectContext context);
    }
}
