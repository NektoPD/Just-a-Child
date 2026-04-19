using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpeechBubbleView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _charDelay = 0.04f;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _delayBetweenSegments = 1.5f;
    [SerializeField] private float _delayAfterText = 2f;

    private Coroutine _coroutine;

    private void Awake()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;
    }

    public void ShowText(string message, Action onComplete)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ShowTextCoroutine(message, onComplete));
    }

    public void ShowTextSegments(string[] segments, Action onComplete)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ShowSegmentsCoroutine(segments, onComplete));
    }

    public void Hide()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
    }

    private IEnumerator ShowTextCoroutine(string message, Action onComplete)
    {
        _text.text = "";
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, _fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);

        var delay = new WaitForSeconds(_charDelay);
        for (int i = 0; i < message.Length; i++)
        {
            _text.text = message.Substring(0, i + 1);
            yield return delay;
        }

        yield return new WaitForSeconds(_delayAfterText);
        _canvasGroup.DOFade(0f, _fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);

        _coroutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator ShowSegmentsCoroutine(string[] segments, Action onComplete)
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, _fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);

        var charWait = new WaitForSeconds(_charDelay);

        for (int s = 0; s < segments.Length; s++)
        {
            _text.text = "";
            string msg = segments[s];
            for (int i = 0; i < msg.Length; i++)
            {
                _text.text = msg.Substring(0, i + 1);
                yield return charWait;
            }

            if (s < segments.Length - 1)
                yield return new WaitForSeconds(_delayBetweenSegments);
        }

        yield return new WaitForSeconds(_delayAfterText);
        _canvasGroup.DOFade(0f, _fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);

        _coroutine = null;
        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        _canvasGroup.DOKill();
    }
}
