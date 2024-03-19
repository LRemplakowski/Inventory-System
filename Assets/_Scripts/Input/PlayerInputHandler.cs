using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using SunsetSystems.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput _input;
        [SerializeField]
        private List<InputActionReference> _disableActionsInUIState = new();

        public static event Action<InputAction.CallbackContext> OnMove;
        public static event Action<InputAction.CallbackContext> OnInventory;

        private void Start()
        {
            _input.actions.actionMaps.ForEach(map => map.Enable());
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

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
