using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public class GhostCreature : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _fleeSpeed = 3f;
    [SerializeField] private float _fleeDistance = 5f;
    [SerializeField] private float _detectRadius = 2.5f;
    [SerializeField] private float _fearOnCollision = 20f;

    private static readonly int Walking = Animator.StringToHash("Walking");

    private Transform _playerTransform;
    private PlayerModel _playerModel;
    private bool _isActive;
    private bool _isFleeing;

    [Inject]
    public void Construct(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }

    public void SetPlayerTransform(Transform player) => _playerTransform = player;

    public void Spawn(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        _isActive = true;
        Debug.Log(_isActive);
        _isFleeing = false;
        SetAlpha(1f);
        _animator.SetFloat(Walking, 0f);
    }

    private void Update()
    {
        if (!_isActive || _isFleeing || _playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, _playerTransform.position);
        
        Debug.Log(dist);
        
        if (dist < _detectRadius)
            StartCoroutine(FleeCoroutine());
    }

    private IEnumerator FleeCoroutine()
    {
        _isFleeing = true;
        Vector3 dir = (transform.position - _playerTransform.position).normalized;
        Vector3 target = transform.position + dir * _fleeDistance;

        if (_spriteRenderer != null)
            _spriteRenderer.flipX = dir.x < 0;

        _animator.SetFloat(Walking, 1f);

        float traveled = 0f;
        while (traveled < _fleeDistance)
        {
            float step = _fleeSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            traveled += step;
            SetAlpha(1f - traveled / _fleeDistance);
            yield return null;
        }

        _animator.SetFloat(Walking, 0f);
        _isActive = false;
        gameObject.SetActive(false);
    }

    private void SetAlpha(float a)
    {
        if (_spriteRenderer == null) return;
        Color c = _spriteRenderer.color;
        c.a = a;
        _spriteRenderer.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive || !other.CompareTag("Player")) return;
        _playerModel.IncreaseFearBy(_fearOnCollision);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
