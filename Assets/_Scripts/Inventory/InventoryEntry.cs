using System;
using SunsetSystems.Inventory.Items;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [Serializable]
    public struct InventoryEntry
    {
        [SerializeField]
        public IBaseItem Item;
        [Min(1)]
        public int StackSize;
    }
}
