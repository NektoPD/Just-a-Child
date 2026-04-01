using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector2 _moveDirection;
    private Rigidbody2D _rigidbody2D;

    private InputAction _moveAction;

    public event Action<float> Moved;
    public event Action<float> MovedX;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        DetectMovement();
    }

    private void DetectMovement()
    {
        _moveDirection = _moveAction.ReadValue<Vector2>();
        
        if(!_rigidbody2D)
            return;

        _rigidbody2D.linearVelocity = _moveDirection * (_speed);
        MovedX?.Invoke(_moveDirection.x);
        Moved?.Invoke(_rigidbody2D.linearVelocity.magnitude);
    }
}
