using System.Collections;
using Furniture;
using UnityEngine;

public class SoundDirectionIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform _arrowIcon;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _showInterval = 3f;
    [SerializeField] private float _showDuration = 1f;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _radius = 80f;

    private Coroutine _indicatorCoroutine;

    public void StartIndicating()
    {
        StopIndicating();
        _canvasGroup.alpha = 0f;
    }

    public void StopIndicating()
    {
        if (_indicatorCoroutine != null)
        {
            StopCoroutine(_indicatorCoroutine);
            _indicatorCoroutine = null;
        }
        _canvasGroup.alpha = 0f;
    }
    
}
