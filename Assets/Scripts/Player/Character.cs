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

    [Inject]
    public void Construct(RoomManager roomManager, ScreenFader screenFader)
    {
        _roomManager = roomManager;
        _screenFader = screenFader;
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public bool TeleportTo(Room targetRoom, Transform targetEntryPoint = null)
    {
        if (targetRoom == null)
        {
            return false;
        }

        if (_roomManager != null && _roomManager.TryTeleport(this, targetRoom, targetEntryPoint))
        {
            return true;
        }

        Transform entryPoint = targetEntryPoint != null ? targetEntryPoint : targetRoom.EntryPoint;
        return ApplyTeleport(entryPoint);
    }

    internal bool ApplyTeleport(Transform entryPoint)
    {
        if (entryPoint == null)
        {
            return false;
        }
        
        _screenFader.FadeIn();

        Vector3 targetPosition = entryPoint.position;

        if (_rigidbody2D != null)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.position = new Vector2(targetPosition.x, targetPosition.y);
        }

        transform.position = targetPosition;
        _screenFader.FadeOut();
        return true;
    }
}
