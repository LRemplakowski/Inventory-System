using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory.Items;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    public class InventoryManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private Dictionary<IBaseItem, InventoryEntry> _items = new();

        public event Action<List<InventoryEntry>> OnInventoryUpdated;

        public void AddItem(IBaseItem item, int count = 0)
        {
            count = count <= 0 ? 1 : count;
            if (_items.TryGetValue(item, out InventoryEntry entry))
            {
                entry.StackSize += count;
            }
            else
            {
                entry = new()
                {
                    Item = item,
                    StackSize = count
                };
            }
            _items[item] = entry;
            OnInventoryUpdated?.Invoke(GetContents());
        }

        public void RemoveItem(IBaseItem item, int count = 0)
        {
            count = count <= 0 ? 1 : count;
            if (_items.TryGetValue(item, out InventoryEntry entry))
            {
                entry.StackSize -= count;
                if (entry.StackSize <= 0)
                    _items.Remove(item);
                else
                    _items[item] = entry;
            }
            OnInventoryUpdated?.Invoke(GetContents());
        }

        public List<InventoryEntry> GetContents()
        {
            return _items.Values.ToList();
        }
    }
}
