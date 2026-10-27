using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private List<RoomData> _roomDatas = new List<RoomData>();
       

        public Transform GetRoomEntryPosition(int id, string name = "name")
        {
            return _roomDatas.FirstOrDefault(room =>
                room.Id == id || room.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))?.EntryPosition;
        }
    }

    [Serializable]
    public class RoomData
    {
        public int Id;
        public string Name;
        public Transform EntryPosition;
    }
}