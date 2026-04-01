using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour
{
    private const string MovementParameter = "Walking";
    
    [SerializeField] private MovementController _movementController;
    
    private int _walking = Animator.StringToHash(MovementParameter);
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _movementController.Moved += SetWalkingAnimation;
    }

    private void OnDisable()
    {
        _movementController.Moved -= SetWalkingAnimation;
    }

    private void SetWalkingAnimation(float speed)
    {
        _animator.SetFloat(_walking, speed);
    }
}