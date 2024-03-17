using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public interface IVisualEffectHandler
    {
        void HandleVisualEffect(IVisualEffect visualEffectPrefab, IVisualEffectSource source, float visualEffectDuration);

        void CancelEffect(IVisualEffectSource source, IVisualEffect instance);
    }
}
