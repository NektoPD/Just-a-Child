using Rooms;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private RoomManager _roomManager;

        public override void InstallBindings()
        {
            Container.Bind<RoomManager>().FromInstance(_roomManager).AsSingle();
        }
    }
}