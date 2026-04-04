using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class IntroTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField, TextArea(3, 10)] private string _introMessage;
    [SerializeField] private float _charDelay = 0.05f;
    [SerializeField] private float _delayAfterFinish = 1f;

    private Coroutine _typewriterCoroutine;

    public void Play(Action onComplete)
    {
        _text.text = "";
        gameObject.SetActive(true);
        _typewriterCoroutine = StartCoroutine(TypewriterCoroutine(onComplete));
    }

    public void Hide()
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator TypewriterCoroutine(Action onComplete)
    {
        var delay = new WaitForSeconds(_charDelay);

        for (int i = 0; i < _introMessage.Length; i++)
        {
            _text.text = _introMessage.Substring(0, i + 1);
            yield return delay;
        }

        yield return new WaitForSeconds(_delayAfterFinish);
        _typewriterCoroutine = null;
        onComplete?.Invoke();
    }
}
