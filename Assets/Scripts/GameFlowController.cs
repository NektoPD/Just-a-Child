using System.Collections;
using DG.Tweening;
using Furniture;
using UniRx;
using UnityEngine;
using Zenject;

public class GameFlowController : MonoBehaviour
{
    [Header("Camera Intro")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _introCameraStart;
    [SerializeField] private Transform _introCameraTarget;
    [SerializeField] private float _introDuration = 3f;

    [Header("Game Timer")]
    [SerializeField] private float _gameDuration = 300f;
    [SerializeField] private TimerView _timerView;

    [Header("Player References")]
    [SerializeField] private MovementController _movementController;
    [SerializeField] private PlayerFearController _fearController;

    [Header("UI")]
    [SerializeField] private CanvasGroup _fearMeterRoot;
    [SerializeField] private CanvasGroup _winScreen;
    [SerializeField] private CanvasGroup _loseScreen;
    [SerializeField] private float _screenFadeDuration = 1f;

    private PlayerModel _playerModel;
    private PlayerConfig _playerConfig;
    private FearAttractionManager _fearAttractionManager;
    private float _remainingTime;
    private bool _gameRunning;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(PlayerModel playerModel, PlayerConfig playerConfig, FearAttractionManager fearAttractionManager)
    {
        _playerModel = playerModel;
        _playerConfig = playerConfig;
        _fearAttractionManager = fearAttractionManager;
    }

    private void Start()
    {
        _movementController.enabled = false;
        _fearMeterRoot.alpha = 0f;
        _timerView.Hide();
        _winScreen.alpha = 0f;
        _winScreen.gameObject.SetActive(false);
        _loseScreen.alpha = 0f;
        _loseScreen.gameObject.SetActive(false);

        _mainCamera.transform.position = _introCameraStart.position;

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        _mainCamera.transform.DOMove(_introCameraTarget.position, _introDuration).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(_introDuration);

        StartGame();
    }

    private void StartGame()
    {
        _movementController.enabled = true;

        _fearMeterRoot.DOFade(1f, 0.5f);
        _timerView.Show();

        _fearController.StartIncreasingFear();
        _fearAttractionManager.StartSpawning();

        _remainingTime = _gameDuration;
        _gameRunning = true;

        _playerModel.CurrentFearLevel.Subscribe(fear =>
        {
            if (_gameRunning && fear >= _playerConfig.MaxFearValue)
                GameOver(false);
        }).AddTo(_disposables);
    }

    private void Update()
    {
        if (!_gameRunning) return;

        _remainingTime -= Time.deltaTime;
        _timerView.UpdateTimer(_remainingTime);

        if (_remainingTime <= 0f)
            GameOver(true);
    }

    private void GameOver(bool won)
    {
        if (!_gameRunning) return;
        _gameRunning = false;

        _fearController.StopIncreasingFear();
        _fearController.StopDecreaseFearWithCooldown();
        _fearAttractionManager.StopSpawning();
        _fearAttractionManager.DeactivateAll();
        _movementController.enabled = false;

        CanvasGroup screen = won ? _winScreen : _loseScreen;
        screen.gameObject.SetActive(true);
        screen.DOFade(1f, _screenFadeDuration);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}
