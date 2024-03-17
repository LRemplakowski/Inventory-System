using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class ItemView : MonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Image _itemIconImage;
        [SerializeField]
        private TextMeshProUGUI _itemNameText;
        [SerializeField]
        private GameObject _itemCountGameObject;
        [SerializeField]
        private TextMeshProUGUI _itemCountText;
        [SerializeField]
        private GameObject _usableItemGameObject;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private InventoryEntry _cachedItemEntry;

        public void OnUseItem()
        {
            if (_cachedItemEntry.Item.ItemType is ItemType.Consumable)
            {
                InventoryUIController.Instance.OnItemUsed(_cachedItemEntry.Item);
                Debug.Log($"Player used item {_cachedItemEntry.Item}!");
            }
        }

        public void UpdateView(InventoryEntry itemEntry)
        {
            gameObject.name = itemEntry.Item.Name;
            _itemCountGameObject.SetActive(itemEntry.Item.Stackable);
            _usableItemGameObject.SetActive(itemEntry.Item.ItemType is ItemType.Consumable);
            _itemIconImage.sprite = itemEntry.Item.Icon;
            _itemNameText.text = itemEntry.Item.Name;
            _itemCountText.text = $"x{itemEntry.StackSize}";
            _cachedItemEntry = itemEntry;
        }
    }
}
