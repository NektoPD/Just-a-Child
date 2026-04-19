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

    [Header("Fade")]
    [SerializeField] private CanvasGroup _fadeOverlay;
    [SerializeField] private float _fadeInDuration = 2f;
    [SerializeField] private float _fadeOutDuration = 1f;

    [Header("Intro Cutscene")]
    [SerializeField] private CutsceneNPC _cutsceneNPC;
    [SerializeField] private SittableObject _startingSeat;
    [SerializeField] private float _cameraBlendWait = 2f;

    [Header("Player Speech")]
    [SerializeField] private SpeechBubbleView _playerSpeechBubble;
    [SerializeField, TextArea(2, 5)] private string _firstFearAttractionText = "What was that sound?..";
    [SerializeField, TextArea(2, 5)] private string _afterInvestigationText = "I need to stay calm..";

    [Header("First Fear Attraction")]
    [SerializeField] private FearAttraction _firstFearAttraction;

    [Header("Lighting")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private float _introLightIntensity = 0.3f;
    [SerializeField] private float _gameLightIntensity = 0.05f;
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

    [Header("Sound Indicator")]
    [SerializeField] private SoundDirectionIndicator _soundIndicator;

    [Header("Random Encounters")]
    [SerializeField] private RandomEncounterManager _encounterManager;

    [Header("Win Cutscene")]
    [SerializeField, TextArea(2, 5)] private string _winCutsceneText = "I survived the night!";
    [SerializeField] private float _winLightIntensity = 1f;

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
        _playerVCam.Priority = 10;
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

        if (_fadeOverlay != null)
            _fadeOverlay.alpha = 1f;

        if (_globalLight != null)
            _globalLight.intensity = _introLightIntensity;

        if (_startingSeat != null)
            _movementController.transform.position = _startingSeat.SitPoint.position;

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        yield return _fadeOverlay.DOFade(0f, _fadeInDuration).WaitForCompletion();

        _introVCam.Priority = 0;
        _playerVCam.Priority = 20;
        yield return new WaitForSeconds(_cameraBlendWait);

        if (_cutsceneNPC != null)
        {
            bool cutsceneDone = false;
            _cutsceneNPC.StartCutscene(() => cutsceneDone = true);
            yield return new WaitUntil(() => cutsceneDone);
        }

        yield return _fadeOverlay.DOFade(1f, _fadeOutDuration).WaitForCompletion();
        yield return new WaitForSeconds(2f);

        if (_firstFearAttraction != null)
            _firstFearAttraction.Activate();

        yield return _fadeOverlay.DOFade(0f, _fadeInDuration).WaitForCompletion();

        bool textDone = false;
        _playerSpeechBubble.ShowText(_firstFearAttractionText, () => textDone = true);
        yield return new WaitUntil(() => textDone);

        _movementController.enabled = true;

        if (_firstFearAttraction != null)
        {
            bool investigated = false;
            void OnInvestigated() => investigated = true;
            _firstFearAttraction.OnInvestigated += OnInvestigated;
            yield return new WaitUntil(() => investigated);
            _firstFearAttraction.OnInvestigated -= OnInvestigated;
        }

        _movementController.enabled = false;
        textDone = false;
        _playerSpeechBubble.ShowText(_afterInvestigationText, () => textDone = true);
        yield return new WaitUntil(() => textDone);

        _movementController.enabled = true;
        StartGame();
    }

    private void StartGame()
    {
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

        if (_soundIndicator != null)
            _soundIndicator.StartIndicating();

        if (_encounterManager != null)
            _encounterManager.StartEncounters();

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

        if (_soundIndicator != null)
            _soundIndicator.StopIndicating();

        if (_encounterManager != null)
            _encounterManager.StopEncounters();

        if (won)
            StartCoroutine(WinCutscene());
        else
        {
            _loseScreen.gameObject.SetActive(true);
            _loseScreen.DOFade(1f, _screenFadeDuration);
            StartCoroutine(RestartAfterDelay());
        }
    }

    private IEnumerator WinCutscene()
    {
        if (_globalLight != null)
            yield return DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x,
                _winLightIntensity, _lightTransitionDuration).WaitForCompletion();

        if (_playerSpeechBubble != null && !string.IsNullOrEmpty(_winCutsceneText))
        {
            bool textDone = false;
            _playerSpeechBubble.ShowText(_winCutsceneText, () => textDone = true);
            yield return new WaitUntil(() => textDone);
        }

        _winScreen.gameObject.SetActive(true);
        _winScreen.DOFade(1f, _screenFadeDuration);
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
