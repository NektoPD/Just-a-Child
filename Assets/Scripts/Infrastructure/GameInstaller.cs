using Rooms;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private RoomManager _roomManager;
        [SerializeField] private ScreenFader _screenFader;

        public override void InstallBindings()
        {
            Container.Bind<RoomManager>().FromInstance(_roomManager).AsSingle();
            Container.Bind<ScreenFader>().FromInstance(_screenFader).AsSingle();
        }
    }
}