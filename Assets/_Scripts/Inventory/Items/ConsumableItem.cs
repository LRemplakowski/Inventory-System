using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Effects;
using UnityEngine;

namespace SunsetSystems.Inventory.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
    public class ConsumableItem : AbstractBaseItem, IConsumable
    {
        public override ItemType ItemType => ItemType.Consumable;

        [field: SerializeField]
        public List<IEffect> ConsumptionEffects { get; private set; } = new();

        public void Use(IEffectHandler effectHandler)
        {
            effectHandler.HandleEffects(ConsumptionEffects);
        }
    }
}
