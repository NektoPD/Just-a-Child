using System.Collections;
using Furniture;
using UnityEngine;

public class SoundDirectionIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform _arrowIcon;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private FearAttractionManager _attractionManager;
    [SerializeField] private float _showInterval = 3f;
    [SerializeField] private float _showDuration = 1f;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _radius = 80f;

    private Coroutine _indicatorCoroutine;

    public void StartIndicating()
    {
        StopIndicating();
        _canvasGroup.alpha = 0f;
        _indicatorCoroutine = StartCoroutine(IndicatorLoop());
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

    private IEnumerator IndicatorLoop()
    {
        while (true)
        {
            FearAttraction active = _attractionManager.GetRandomActive();
            if (active != null)
            {
                Vector3 dir = (active.transform.position - _playerTransform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _arrowIcon.localRotation = Quaternion.Euler(0, 0, angle);
                _arrowIcon.anchoredPosition = new Vector2(dir.x, dir.y) * _radius;

                float t = 0;
                while (t < _fadeDuration)
                {
                    t += Time.deltaTime;
                    _canvasGroup.alpha = t / _fadeDuration;
                    yield return null;
                }
                _canvasGroup.alpha = 1f;

                yield return new WaitForSeconds(_showDuration);

                t = 0;
                while (t < _fadeDuration)
                {
                    t += Time.deltaTime;
                    _canvasGroup.alpha = 1f - t / _fadeDuration;
                    yield return null;
                }
                _canvasGroup.alpha = 0f;
            }

            yield return new WaitForSeconds(_showInterval);
        }
    }
}
