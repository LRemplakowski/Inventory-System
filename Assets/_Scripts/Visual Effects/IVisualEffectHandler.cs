using UnityEngine;

namespace SunsetSystems.VisualEffects
{
    public interface IVisualEffectHandler
    {
        void HandleVisualEffect(GameObject visualEffectPrefab, IVisualEffectSource source, float visualEffectDuration);

        void CancelEffect(IVisualEffectSource source, GameObject prefab);
    }
}
