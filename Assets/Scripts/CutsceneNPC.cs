using System;
using System.Collections;
using UnityEngine;

public class CutsceneNPC : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpeechBubbleView _speechBubble;
    [SerializeField] private Transform _walkTarget;
    [SerializeField] private Transform _exitTarget;
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField, TextArea(2, 5)] private string[] _dialogSegments = new string[3];

    private static readonly int Walking = Animator.StringToHash("Walking");

    public void StartCutscene(Action onComplete)
    {
        gameObject.SetActive(true);
        StartCoroutine(CutsceneCoroutine(onComplete));
    }

    private IEnumerator CutsceneCoroutine(Action onComplete)
    {
        _animator.SetFloat(Walking, 1f);
        FlipTowards(_walkTarget.position);
        yield return StartCoroutine(WalkTo(_walkTarget.position));
        _animator.SetFloat(Walking, 0f);

        bool speechDone = false;
        _speechBubble.ShowTextSegments(_dialogSegments, () => speechDone = true);
        yield return new WaitUntil(() => speechDone);

        _animator.SetFloat(Walking, 1f);
        FlipTowards(_exitTarget.position);
        yield return StartCoroutine(WalkTo(_exitTarget.position));
        _animator.SetFloat(Walking, 0f);

        _speechBubble.Hide();
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    private IEnumerator WalkTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _walkSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    private void FlipTowards(Vector3 target)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.flipX = target.x < transform.position.x;
    }
}
