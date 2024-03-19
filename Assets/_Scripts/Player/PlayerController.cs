using Sirenix.OdinInspector;
using SunsetSystems.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Player
{
    public class PlayerController : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField, Required]
        private CharacterController _characterController;
        [SerializeField, Required]
        private Transform _playerCameraTransform;

        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _playerMoveSpeed = 2f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Vector2 _playerMoveVector = Vector2.zero;

        private void OnEnable()
        {
            PlayerInputHandler.OnMove += OnMoveAction;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnMove -= OnMoveAction;
        }

        private void Update()
        {
            _characterController.transform.rotation = Quaternion.identity;
            MovePlayer(Time.deltaTime);
        }

        private void MovePlayer(float deltaTime)
        {
            Vector3 movementThisFrame = InputMovementVectorToCharacterMovementVector(_playerMoveVector);
            movementThisFrame = _playerMoveSpeed * deltaTime * movementThisFrame;
            _characterController.Move(movementThisFrame);
        }

        private Vector3 InputMovementVectorToCharacterMovementVector(Vector2 moveInput)
        {
            Vector3 resultMoveVector = new(moveInput.x, 0, moveInput.y);
            resultMoveVector = _characterController.transform.InverseTransformDirection(_playerCameraTransform.TransformDirection(resultMoveVector));
            resultMoveVector.y = 0;
            return resultMoveVector.normalized;
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            _playerMoveVector = context.ReadValue<Vector2>().normalized;
        }
    }
}
