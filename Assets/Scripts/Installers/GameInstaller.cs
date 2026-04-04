using Player;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _playerView;

        public override void InstallBindings()
        {
            Container.Bind<IPlayerView>().FromInstance(_playerView).AsSingle();
            Container.Bind<PlayerModel>().AsSingle().NonLazy();
            
        }
    }
}