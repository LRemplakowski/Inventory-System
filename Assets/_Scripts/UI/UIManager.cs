using Sirenix.OdinInspector;
using SunsetSystems.Core;
using SunsetSystems.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class UIManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private InventoryUIController _inventoryUI;

        private void OnEnable()
        {
            InputManager.OnInventory += ToggleInventory;
        }

        private void OnDisable()
        {
            InputManager.OnInventory -= ToggleInventory;
        }

        private void ToggleInventory(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (_inventoryUI.gameObject.activeInHierarchy)
            {
                _inventoryUI.gameObject.SetActive(false);
                GameManager.Instance.State = GameState.Game;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _inventoryUI.gameObject.SetActive(true);
                GameManager.Instance.State = GameState.UI;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}
