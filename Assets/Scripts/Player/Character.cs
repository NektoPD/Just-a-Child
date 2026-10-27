using Rooms;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
    {
        private RoomManager _roomManager;

        [Inject]
        public void Construct(RoomManager roomManager)
        {
            _roomManager = roomManager;
        }
        
        
    }