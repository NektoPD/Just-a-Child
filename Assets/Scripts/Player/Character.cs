using Cysharp.Threading.Tasks;
using Rooms;
using UI;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    private RoomManager _roomManager;
    private ScreenFader _screenFader;
    private Rigidbody2D _rigidbody2D;
    private MovementController _movementController;

    public bool IsTeleporting { get; private set; }

    [Inject]
    public void Construct(RoomManager roomManager, ScreenFader screenFader)
    {
        _roomManager = roomManager;
        _screenFader = screenFader;
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementController = GetComponent<MovementController>();
    }

    public async UniTask<bool> TeleportToAsync(Room targetRoom, Transform targetEntryPoint = null)
    {
        if (targetRoom == null || IsTeleporting)
        {
            return false;
        }

        if (_screenFader == null)
        {
            Debug.LogWarning("Assign ScreenFader in GameInstaller before teleporting.", this);
            return false;
        }

        Transform entryPoint = targetEntryPoint != null ? targetEntryPoint : targetRoom.EntryPoint;
        IsTeleporting = true;
        bool movementWasEnabled = _movementController != null && _movementController.enabled;

        try
        {
            if (_movementController != null)
            {
                _movementController.enabled = false;
            }

            if (_rigidbody2D != null)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
                _rigidbody2D.angularVelocity = 0f;
            }

            await _screenFader.FadeInAsync();

            if (this == null || _screenFader == null)
            {
                return false;
            }

            bool teleported = _roomManager != null
                ? _roomManager.TryTeleport(this, targetRoom, entryPoint)
                : ApplyTeleport(entryPoint);

            await _screenFader.FadeOutAsync();
            return teleported;
        }
        finally
        {
            if (_movementController != null)
            {
                _movementController.enabled = movementWasEnabled;
            }

            IsTeleporting = false;
        }
    }

    internal bool ApplyTeleport(Transform entryPoint)
    {
        if (entryPoint == null)
        {
            return false;
        }

        Vector3 targetPosition = entryPoint.position;

        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.position = new Vector2(targetPosition.x, targetPosition.y);
        }

        transform.position = targetPosition;
        return true;
    }
}
