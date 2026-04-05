using System.Collections;
using Cinemachine;
using DG.Tweening;
using Furniture;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Zenject;

public class GameFlowController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera _introVCam;
    [SerializeField] private CinemachineVirtualCamera _playerVCam;

    [Header("Intro Text")]
    [SerializeField] private IntroTextView _introTextView;

    [Header("Lighting")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private float _introLightIntensity = 0.3f;
    [SerializeField] private float _gameLightIntensity = 0.15f;
    [SerializeField] private float _lightTransitionDuration = 1.5f;

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
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private CanvasGroup _controlsHint;

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

    private void Awake()
    {
        _introVCam.Priority = 20;
        _playerVCam.Priority = 0;
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

        if (_controlsHint != null)
        {
            _controlsHint.alpha = 0f;
            _controlsHint.gameObject.SetActive(false);
        }

        if (_globalLight != null)
            _globalLight.intensity = _introLightIntensity;

        _introTextView.Play(OnIntroTextFinished);
    }

    private void OnIntroTextFinished()
    {
        _introTextView.Hide();
        _introVCam.Priority = 0;
        _playerVCam.Priority = 20;

        StartGame();
    }

    private void StartGame()
    {
        _movementController.enabled = true;

        _fearMeterRoot.DOFade(1f, 0.5f);
        _timerView.Show();

        if (_controlsHint != null)
        {
            _controlsHint.gameObject.SetActive(true);
            _controlsHint.DOFade(1f, 0.3f).OnComplete(() =>
                _controlsHint.DOFade(0f, 0.5f).SetDelay(2f));
        }

        if (_globalLight != null)
            DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x,
                _gameLightIntensity, _lightTransitionDuration);

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

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(_restartDelay);
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}
