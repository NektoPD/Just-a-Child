using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour
{
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    [SerializeField] private MovementController _movementController;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _movementController.Moved += OnMoved;
        _movementController.DirectionChanged += OnDirectionChanged;
        _movementController.MovingChanged += OnMovingChanged;
    }

    private void OnDisable()
    {
        _movementController.Moved -= OnMoved;
        _movementController.DirectionChanged -= OnDirectionChanged;
        _movementController.MovingChanged -= OnMovingChanged;
    }

    private void OnMoved(float speed)
    {
    }

    private void OnDirectionChanged(Vector2 rawInput)
    {
        _animator.SetFloat(MoveXHash, rawInput.x);
        _animator.SetFloat(MoveYHash, rawInput.y);
    }

    private void OnMovingChanged(bool isMoving)
    {
        _animator.SetBool(IsMovingHash, isMoving);
    }
}