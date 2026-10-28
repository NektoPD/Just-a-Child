using Cysharp.Threading.Tasks;
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
            if (_playerInReach == null || _playerInReach.IsTeleporting || Mouse.current == null ||
                !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            TeleportPlayerAsync().Forget();
        }

        private void SetPlayerIfFound(Collider2D other)
        {
            Character character = other.GetComponentInParent<Character>();

            if (character != null)
            {
                _playerInReach = character;
            }
        }

        private async UniTask TeleportPlayerAsync()
        {
            if (_targetRoom == null)
            {
                Debug.LogWarning("RoomTeleport has no target Room assigned.", this);
                return;
            }

            Character player = _playerInReach;

            if (!await player.TeleportToAsync(_targetRoom, _targetEntryPoint))
            {
                Debug.LogWarning("RoomTeleport could not teleport the player.", this);
            }
        }
    }
}
