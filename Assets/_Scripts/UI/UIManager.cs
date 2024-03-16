using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Input;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class UIManager : SerializedMonoBehaviour
    {
        private bool _uiEnabled = false;

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

        }
    }
}
