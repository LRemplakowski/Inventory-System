using UnityEngine;

namespace SunsetSystems.Inventory.Items
{
    public interface IBaseItem
    {
        string Name { get; }
        ItemType ItemType { get; }
        bool Stackable { get; }
        Sprite Icon { get; }
    }
}
