using UnityEngine;
using UnityEngine.InputSystem;

namespace Rooms
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class RoomTeleport : MonoBehaviour
    {
        [SerializeField] private Room _targetRoom;
        [SerializeField] private Transform _targetEntryPoint;
        [SerializeField] private Camera _inputCamera;

        private Collider2D _collider;
        private Character _playerInReach;

        public Room TargetRoom => _targetRoom;
        public Transform TargetEntryPoint => _targetEntryPoint != null
            ? _targetEntryPoint
            : _targetRoom != null ? _targetRoom.EntryPoint : null;
        public bool PlayerInReach => _playerInReach != null;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();

            if (!_collider.isTrigger)
            {
                Debug.LogWarning("RoomTeleport requires a trigger Collider2D.", this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            SetPlayerIfFound(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            SetPlayerIfFound(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Character character = other.GetComponentInParent<Character>();

            if (character == _playerInReach)
            {
                _playerInReach = null;
            }
        }

        private void Update()
        {
            if (_playerInReach == null || Mouse.current == null ||
                !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (!IsPointerOverCollider())
            {
                return;
            }

            TeleportPlayer();
        }

        private void SetPlayerIfFound(Collider2D other)
        {
            Character character = other.GetComponentInParent<Character>();

            if (character != null)
            {
                _playerInReach = character;
            }
        }

        private bool IsPointerOverCollider()
        {
            if (_collider == null || Mouse.current == null)
            {
                return false;
            }

            Camera camera = _inputCamera != null ? _inputCamera : Camera.main;

            if (camera == null)
            {
                return false;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            float distanceToCollider = Mathf.Abs(camera.transform.position.z - _collider.transform.position.z);
            Vector3 worldPosition = camera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, distanceToCollider));

            return _collider.OverlapPoint(worldPosition);
        }

        private void TeleportPlayer()
        {
            if (_targetRoom == null)
            {
                Debug.LogWarning("RoomTeleport has no target Room assigned.", this);
                return;
            }

            if (!_playerInReach.TeleportTo(_targetRoom, _targetEntryPoint))
            {
                Debug.LogWarning("RoomTeleport could not find a target entry point.", this);
            }
        }
    }
}
