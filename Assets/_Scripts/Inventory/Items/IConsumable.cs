using System.Collections.Generic;
using SunsetSystems.Effects;

namespace SunsetSystems.Inventory.Items
{
    public interface IConsumable
    {
        List<IEffect> ConsumptionEffects { get; }

        void Use(IEffectHandler effectHandler);
    }
}
