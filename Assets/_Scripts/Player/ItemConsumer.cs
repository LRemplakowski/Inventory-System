using Sirenix.OdinInspector;
using SunsetSystems.Effects;
using SunsetSystems.Inventory.Items;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    public class ItemConsumer : SerializedMonoBehaviour
    {
        [SerializeField]
        private IEffectHandler _effectHandler;

        public bool UseItem(IBaseItem item)
        {
            if (item is IConsumable consumable)
                return UseConsumable(consumable);
            else
                return false;
        }

        public bool UseConsumable(IConsumable consumable)
        {
            consumable.Use(_effectHandler);
            return true;
        }
    }
}
