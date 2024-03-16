using System.Collections.Generic;
using Sirenix.Utilities;

namespace SunsetSystems.Effects
{
    public interface IEffectHandler
    {
        void HandleEffect(IEffect effect);

        void HandleEffects(IEnumerable<IEffect> effects) => effects.ForEach(effect => HandleEffect(effect));
    }
}
