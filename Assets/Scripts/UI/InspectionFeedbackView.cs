using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InspectionFeedbackView : MonoBehaviour
{
    [Header("Vignette (Volume on Player VCam)")]
    [SerializeField] private Volume _postProcessVolume;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _vignetteDuration = 0.5f;
    [SerializeField] private float _successVignetteIntensity = 0.45f;
    [SerializeField] private float _failVignetteIntensity = 0.55f;

    [Header("Feedback Text")]
    [SerializeField] private TMP_Text _feedbackText;
    [SerializeField] private string _successMessage = "Wuhh";

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _shakeForce = 0.3f;

    [Header("Colors")]
    [SerializeField] private Color _successColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color _failColor = new Color(1f, 0.15f, 0.1f);

    private Vignette _vignette;
    private Tween _vignetteTween;
    private Tween _textTween;

    private void Awake()
    {
        _feedbackText.alpha = 0f;

        if (_mainCamera != null)
        {
            var cameraData = _mainCamera.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }

        if (_postProcessVolume != null && _postProcessVolume.profile.TryGet(out _vignette))
        {
            _vignette.intensity.overrideState = true;
            _vignette.color.overrideState = true;
            _vignette.intensity.value = 0f;
        }
    }

    public void PlaySuccess()
    {
        KillAll();

        _feedbackText.text = _successMessage;
        _feedbackText.alpha = 1f;
        _textTween = DOTween.ToAlpha(() => _feedbackText.color, c => _feedbackText.color = c, 0f, _vignetteDuration)
            .SetDelay(_vignetteDuration * 0.5f);

        AnimateVignette(_successColor, _successVignetteIntensity);
    }

    public void PlayFail()
    {
        KillAll();

        _feedbackText.alpha = 0f;

        AnimateVignette(_failColor, _failVignetteIntensity);

        if (_impulseSource != null)
            _impulseSource.GenerateImpulse(_shakeForce);
    }

    private void AnimateVignette(Color color, float intensity)
    {
        if (_vignette == null) return;

        _vignette.color.value = color;
        _vignette.intensity.value = intensity;

        _vignetteTween = DOTween.To(
            () => _vignette.intensity.value,
            x => _vignette.intensity.value = x,
            0f,
            _vignetteDuration
        );
    }

    private void KillAll()
    {
        _vignetteTween?.Kill();
        _textTween?.Kill();
    }

    private void OnDestroy()
    {
        KillAll();
    }
}
