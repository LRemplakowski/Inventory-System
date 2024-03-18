using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Items;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        public static InventoryUIController Instance { get; private set; }

        [SerializeField, Required, SceneObjectsOnly]
        private InventoryManager _playerInventory;
        [SerializeField, Required, SceneObjectsOnly]
        private ItemConsumer _itemConsumer;
        [SerializeField, Required, SceneObjectsOnly]
        private Transform _viewParent;
        [SerializeField, Required, AssetsOnly]
        private ItemView _itemViewPrefab;

        private readonly Queue<ItemView> _itemViewPool = new();
        private readonly List<ItemView> _viewsInUse = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            OnInventoryUpdated(_playerInventory.GetContents());
            _playerInventory.OnInventoryUpdated += OnInventoryUpdated;
        }

        private void OnDisable()
        {
            _playerInventory.OnInventoryUpdated -= OnInventoryUpdated;
        }

        // Rebuilding all views on content update is inefficient, but will do for the purpose of this test task.
        private void OnInventoryUpdated(List<InventoryEntry> contents)
        {
            _viewsInUse.ForEach(view => ReturnViewToPool(view));
            _viewsInUse.Clear();
            foreach (InventoryEntry entry in contents)
            {
                if (entry.Item.Stackable)
                {
                    SetUpView(entry);
                }
                else
                {
                    for (int i = 0; i < entry.StackSize; i++)
                    {
                        SetUpView(entry);
                    }
                }
            }

            void SetUpView(InventoryEntry entry)
            {
                ItemView view = GetViewFromPool();
                view.UpdateView(entry);
                _viewsInUse.Add(view);
            }
        }

        private ItemView GetViewFromPool()
        {
            if (_itemViewPool.TryDequeue(out ItemView result))
            {
                result.gameObject.SetActive(true);
            }
            else
            {
                result = Instantiate(_itemViewPrefab, _viewParent);
                result.gameObject.SetActive(true);
            }
            return result;
        }

        private void ReturnViewToPool(ItemView view)
        {
            view.gameObject.SetActive(false);
            _itemViewPool.Enqueue(view);
        }

        public void OnItemUsed(IBaseItem item)
        {
            if (_itemConsumer.UseItem(item))
                _playerInventory.RemoveItem(item);
            else
                Debug.LogError($"Item {item} is not IConsumable! It cannot be used!");
        }
    }
}
