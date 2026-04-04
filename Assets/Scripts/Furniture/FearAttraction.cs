using System;
using System.Collections;
using DG.Tweening;
using Player;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Furniture
{
    [RequireComponent(typeof(Collider2D))]
    public class FearAttraction : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _sounds;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _fearPerTick = 2f;
        [SerializeField] private float _tickInterval = 3f;
        [SerializeField] private float _shakeStrength = 0.1f;
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeInterval = 2f;
        [SerializeField] private float _fearReductionOnInvestigate = 15f;
        [SerializeField] private FearDecreaseArea _linkedDecreaseArea;

        public FearDecreaseArea LinkedDecreaseArea => _linkedDecreaseArea;

        private PlayerModel _playerModel;
        private IPlayerView _playerView;
        private InputController _inputController;
        private InspectionFeedbackView _feedbackView;
        private Coroutine _fearCoroutine;
        private Tween _shakeTween;
        private Vector3 _originalPosition;
        private bool _playerInRange;
        private IDisposable _interactionSub;

        public bool IsActive { get; private set; }
        public event Action OnInvestigated;

        [Inject]
        public void Construct(PlayerModel playerModel, IPlayerView playerView, InputController inputController, InspectionFeedbackView feedbackView)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _inputController = inputController;
            _feedbackView = feedbackView;
        }

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        private void Start()
        {
            _interactionSub = _inputController.OnInteractionPerformed.Subscribe(_ =>
            {
                if (_playerInRange && IsActive)
                    Investigate();
            });

            _linkedDecreaseArea = GetComponentInChildren<FearDecreaseArea>();
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;

            if (_audioSource != null && _sounds.Length > 0)
            {
                _audioSource.clip = _sounds[Random.Range(0, _sounds.Length)];
                _audioSource.loop = true;
                _audioSource.Play();
            }

            StartShakeLoop();
            _fearCoroutine = StartCoroutine(FearTickCoroutine());
        }

        public void Investigate()
        {
            if (!IsActive) return;
            Deactivate();
            _playerModel.DecreaseFearBy(_fearReductionOnInvestigate);
            _feedbackView.PlaySuccess();
            OnInvestigated?.Invoke();
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;

            if (_audioSource != null)
                _audioSource.Stop();

            _shakeTween?.Kill();
            transform.localPosition = _originalPosition;

            if (_fearCoroutine != null)
            {
                StopCoroutine(_fearCoroutine);
                _fearCoroutine = null;
            }

            if (_playerInRange)
                _playerView.ShowInteraction(false);
        }

        private void StartShakeLoop()
        {
            _shakeTween?.Kill();
            _shakeTween = transform.DOShakePosition(_shakeDuration, _shakeStrength)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(_shakeInterval);
        }

        private IEnumerator FearTickCoroutine()
        {
            var wait = new WaitForSeconds(_tickInterval);
            while (IsActive)
            {
                yield return wait;
                if (IsActive)
                    _playerModel.IncreaseFearBy(_fearPerTick);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActive) return;
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
            _shakeTween?.Kill();
            _interactionSub?.Dispose();
            if (_fearCoroutine != null)
                StopCoroutine(_fearCoroutine);
        }
    }
}