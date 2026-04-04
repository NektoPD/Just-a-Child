using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerView;

        private InputAction _interactAction;
        private readonly Subject<Unit> _interactionPerformed = new();

        public IObservable<Unit> OnInteractionPerformed => _interactionPerformed;

        private void Awake()
        {
            _interactAction = InputSystem.actions.FindAction("Interact");

            _interactAction.Enable();

            _interactAction.performed += OnInteractPerformed;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (_playerView.InteactionAvailable)
            {
                _interactionPerformed.OnNext(Unit.Default);
            }
            
        }

        private void OnDestroy()
        {
            if (_interactAction != null)
            {
                _interactAction.performed -= OnInteractPerformed;
                _interactAction.Disable();
            }

            _interactionPerformed?.Dispose();
        }
    }
}