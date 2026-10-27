using System.Collections.Generic;
using UnityEngine;

namespace Rooms
{
    public class Room : MonoBehaviour
    {
        [Header("Room")]
        [SerializeField] private int _id;
        [SerializeField] private string _roomName;
        [SerializeField] private Transform _entryPoint;

        [Header("Teleports")]
        [SerializeField] private List<RoomTeleport> _teleports = new List<RoomTeleport>();

        public int Id => _id;
        public string RoomName => string.IsNullOrWhiteSpace(_roomName) ? name : _roomName;
        public Transform EntryPoint => _entryPoint != null ? _entryPoint : transform;
        public IReadOnlyList<RoomTeleport> Teleports => _teleports;

        private void Reset()
        {
            _teleports = new List<RoomTeleport>(GetComponentsInChildren<RoomTeleport>(true));
        }
    }
}
