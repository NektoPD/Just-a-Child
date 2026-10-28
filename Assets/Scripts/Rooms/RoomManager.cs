using System.Collections.Generic;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private List<Room> _rooms = new List<Room>();

        public IReadOnlyList<Room> Rooms => _rooms;

        public bool TryGetRoom(int id, out Room room)
        {
            room = _rooms.Find(item => item != null && item.Id == id);
            return room != null;
        }

        public bool TryGetRoom(string roomName, out Room room)
        {
            room = _rooms.Find(item => item != null &&
                string.Equals(item.RoomName, roomName, System.StringComparison.InvariantCultureIgnoreCase));
            return room != null;
        }

        public bool TryTeleport(Character player, Room targetRoom, Transform targetEntryPoint)
        {
            if (player == null || targetRoom == null)
            {
                return false;
            }

            Transform entryPoint = targetEntryPoint != null ? targetEntryPoint : targetRoom.EntryPoint;
            return player.ApplyTeleport(entryPoint);
        }
    }
}
