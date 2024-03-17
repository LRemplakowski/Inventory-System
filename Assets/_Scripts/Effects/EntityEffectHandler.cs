using Sirenix.OdinInspector;
using SunsetSystems.VisualEffects;
using UnityEngine;

namespace SunsetSystems.Effects
{
    public class EntityEffectHandler : SerializedMonoBehaviour, IEffectHandler
    {
        [SerializeField]
        private VisualEffectHandler _vfxHandler;

        public void HandleEffect(IEffect effect)
        {
            EffectContext context = new(this, _vfxHandler, effect);
            effect.ApplyEffect(context);
        }
    }
}
