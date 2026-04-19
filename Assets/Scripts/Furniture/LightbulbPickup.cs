using Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Furniture
{
    [RequireComponent(typeof(Collider2D))]
    public class LightbulbPickup : MonoBehaviour
    {
        [SerializeField] private FearDecreasingLightSource _targetLightSource;

        private IPlayerView _playerView;
        private InputController _inputController;
        private bool _playerInRange;
        private bool _collected;
        private IDisposable _interactionSub;

        [Inject]
        public void Construct(IPlayerView playerView, InputController inputController)
        {
            _playerView = playerView;
            _inputController = inputController;
        }

        private void Start()
        {
            _interactionSub = _inputController.OnInteractionPerformed.Subscribe(_ =>
            {
                if (!_playerInRange || _collected) return;
                Collect();
            });
        }

        private void Collect()
        {
            _collected = true;
            _playerView.ShowInteraction(false);

            if (_targetLightSource != null)
                _targetLightSource.EnableLight();

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected || !other.CompareTag("Player")) return;
            _playerInRange = true;
            _playerView.ShowInteraction(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
            _playerView.ShowInteraction(false);
        }

        private void OnDestroy()
        {
            _interactionSub?.Dispose();
        }
    }
}
