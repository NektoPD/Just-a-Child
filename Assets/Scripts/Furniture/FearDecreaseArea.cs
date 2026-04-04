using System;
using Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Furniture
{
    [RequireComponent(typeof(Collider2D))]
    public class FearDecreaseArea : MonoBehaviour
    {
        [SerializeField] private FearAttraction _linkedAttraction;
        [SerializeField] private float _fearIncreaseOnWrongInspect = 10f;

        private IPlayerView _playerView;
        private PlayerModel _playerModel;
        private InputController _inputController;
        private InspectionFeedbackView _feedbackView;
        private bool _playerInRange;
        private IDisposable _interactionSub;

        [Inject]
        public void Construct(IPlayerView playerView, PlayerModel playerModel, InputController inputController, InspectionFeedbackView feedbackView)
        {
            _playerView = playerView;
            _playerModel = playerModel;
            _inputController = inputController;
            _feedbackView = feedbackView;
        }

        private void Start()
        {
            _interactionSub = _inputController.OnInteractionPerformed.Subscribe(_ =>
            {
                if (!_playerInRange) return;

                if (_linkedAttraction != null && _linkedAttraction.IsActive)
                {
                    _linkedAttraction.Investigate();
                }
                else
                {
                    _playerModel.IncreaseFearBy(_fearIncreaseOnWrongInspect);
                    _feedbackView.PlayFail();
                }
            });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _playerInRange = true;
            _playerView.ShowInteraction(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _playerInRange = false;
            _playerView.ShowInteraction(false);
        }

        private void OnDestroy()
        {
            _interactionSub?.Dispose();
        }
    }
}