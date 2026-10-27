using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector2 _moveDirection;
    private Vector2 _lastInputDirection = Vector2.down;
    private Rigidbody2D _rigidbody2D;

    private InputAction _moveAction;

    public event Action<float> Moved;
    public event Action<Vector2> DirectionChanged;
    public event Action<bool> MovingChanged;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        _moveDirection = _moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        DetectMovement(_moveDirection);
    }

    private void OnDisable()
    {
        _moveDirection = Vector2.zero;

        if (_rigidbody2D != null)
            _rigidbody2D.linearVelocity = Vector2.zero;

        Moved?.Invoke(0f);
    }

    private void DetectMovement(Vector2 input)
    {
        if (!_rigidbody2D) return;

        bool isMoving = input.sqrMagnitude >= 0.01f;

        if (!isMoving)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            Moved?.Invoke(0f);
            MovingChanged?.Invoke(false);
            return;
        }

        _lastInputDirection = input.normalized;

        Vector2 outputDirection = NormalizeIsometricMovement(input);
        if (outputDirection == Vector2.zero)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            Moved?.Invoke(0f);
            MovingChanged?.Invoke(false);
            return;
        }

        _rigidbody2D.linearVelocity = outputDirection * _speed;
        Moved?.Invoke(_rigidbody2D.linearVelocity.magnitude);
        MovingChanged?.Invoke(true);
        DirectionChanged?.Invoke(input);
    }

    private Vector2 NormalizeIsometricMovement(Vector2 inputVector)
    {
        int x = Mathf.Abs(inputVector.x) < 0.3f ? 0 : (inputVector.x > 0 ? 1 : -1);
        int y = Mathf.Abs(inputVector.y) < 0.3f ? 0 : (inputVector.y > 0 ? 1 : -1);
        
        if (x == 0 && y == 0) return Vector2.zero;
        
        if (x == 0 || y == 0) return new Vector2(x, y);
        return new Vector2(x, y * 0.5f);
    }
}
