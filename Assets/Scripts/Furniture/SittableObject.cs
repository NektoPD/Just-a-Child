using System.Collections;
using Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Furniture
{
    [RequireComponent(typeof(Collider2D))]
    public class SittableObject : MonoBehaviour
    {
        [SerializeField] private Transform _sitPoint;
        [SerializeField] private float _fearDecreasePerTick = 1f;
        [SerializeField] private float _fearDecreaseInterval = 2f;

        private PlayerModel _playerModel;
        private IPlayerView _playerView;
        private InputController _inputController;
        private MovementController _movementController;
        private bool _playerInRange;
        private bool _isOccupied;
        private IDisposable _interactionSub;
        private Coroutine _decreaseCoroutine;

        public bool IsOccupied => _isOccupied;
        public Transform SitPoint => _sitPoint != null ? _sitPoint : transform;

        [Inject]
        public void Construct(PlayerModel playerModel, IPlayerView playerView, InputController inputController, MovementController movementController)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _inputController = inputController;
            _movementController = movementController;
        }

        private void Start()
        {
            _interactionSub = _inputController.OnInteractionPerformed.Subscribe(_ =>
            {
                if (!_playerInRange) return;
                if (_isOccupied) StandUp();
                else SitDown();
            });
        }

        public void SitDown()
        {
            _isOccupied = true;
            _movementController.transform.position = SitPoint.position;
            _movementController.enabled = false;
            _playerModel.SetSitting(true);
            _decreaseCoroutine = StartCoroutine(FearDecreaseCoroutine());
        }

        public void StandUp()
        {
            _isOccupied = false;
            _movementController.enabled = true;
            _playerModel.SetSitting(false);
            if (_decreaseCoroutine != null)
            {
                StopCoroutine(_decreaseCoroutine);
                _decreaseCoroutine = null;
            }
        }

        private IEnumerator FearDecreaseCoroutine()
        {
            var wait = new WaitForSeconds(_fearDecreaseInterval);
            while (_isOccupied)
            {
                _playerModel.DecreaseFearBy(_fearDecreasePerTick);
                yield return wait;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = true;
            _playerView.ShowInteraction(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
            if (!_isOccupied)
                _playerView.ShowInteraction(false);
        }

        private void OnDestroy()
        {
            _interactionSub?.Dispose();
            if (_decreaseCoroutine != null)
                StopCoroutine(_decreaseCoroutine);
        }
    }
}
