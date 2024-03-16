using System;
using System.Collections.Generic;
using SunsetSystems.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private List<InputActionReference> _disableActionsInUIState = new();

        public static event Action<InputAction.CallbackContext> OnMove;
        public static event Action<InputAction.CallbackContext> OnInventory;

        public void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Game:
                    _disableActionsInUIState.ForEach(actionReference => actionReference.action.Enable());
                    break;
                case GameState.UI:
                    _disableActionsInUIState.ForEach(actionReference => actionReference.action.Disable());
                    break;
            }
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context);
        }

        public void OnInventoryAction(InputAction.CallbackContext context)
        {
            OnInventory?.Invoke(context);
        }
    }
}
