using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private Transform _startingPoint;
    [SerializeField] private float _cameraBlendWait = 2f;

    [Header("Lighting")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private float _introLightIntensity = 0.3f;
    [SerializeField] private float _gameLightIntensity = 0.05f;
    [SerializeField] private float _lightTransitionDuration = 1.5f;

    [Header("Character References")]
    [SerializeField] private MovementController _movementController;

    private Tween _lightTween;

    private void Awake()
    {
        if (_introVCam != null)
            _introVCam.Priority = 20;

        if (_playerVCam != null)
            _playerVCam.Priority = 10;
    }

    private void Start()
    {
        _movementController.enabled = false;

        if (_fadeOverlay != null)
            _fadeOverlay.alpha = 1f;

        if (_globalLight != null)
            _globalLight.intensity = _introLightIntensity;

        if (_startingPoint != null)
            _movementController.transform.position = _startingPoint.position;

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        if (_fadeOverlay != null)
            yield return _fadeOverlay.DOFade(0f, _fadeInDuration).WaitForCompletion();

        if (_introVCam != null)
            _introVCam.Priority = 0;

        if (_playerVCam != null)
            _playerVCam.Priority = 20;

        yield return new WaitForSeconds(_cameraBlendWait);

        if (_cutsceneNPC != null)
        {
            bool cutsceneDone = false;
            _cutsceneNPC.StartCutscene(() => cutsceneDone = true);
            yield return new WaitUntil(() => cutsceneDone);
        }

        if (_fadeOverlay != null)
        {
            yield return _fadeOverlay.DOFade(1f, _fadeOutDuration).WaitForCompletion();
            yield return new WaitForSeconds(2f);
            yield return _fadeOverlay.DOFade(0f, _fadeInDuration).WaitForCompletion();
        }

        if (_globalLight != null)
        {
            _lightTween = DOTween.To(
                () => _globalLight.intensity,
                intensity => _globalLight.intensity = intensity,
                _gameLightIntensity,
                _lightTransitionDuration);
        }

        _movementController.enabled = true;
    }

    private void OnDestroy()
    {
        _lightTween?.Kill();

        if (_fadeOverlay != null)
            _fadeOverlay.DOKill();
    }
}
