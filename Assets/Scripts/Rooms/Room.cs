using System;
using UnityEngine;

namespace Rooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Collider2D _exitCollider;
        
        public bool PlayerInReach { get; private set; }

        private RoomData _data;

        public event Action GotPlayer;
        
        private void OnTriggerStay2D(Collider2D other)
        { 
            GotPlayer?.Invoke();
        }

        public void SetupData(RoomData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}