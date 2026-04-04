using Furniture;
using Player;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private PlayerFearController _playerFearController;
        [SerializeField] private InputController _inputController;
        [SerializeField] private GameFlowController _gameFlowController;
        [SerializeField] private FearAttractionManager _fearAttractionManager;

        public override void InstallBindings()
        {
            Container.Bind<IPlayerView>().FromInstance(_playerView).AsSingle();
            Container.Bind<PlayerModel>().AsSingle().NonLazy();
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
            Container.Bind<PlayerFearController>().FromInstance(_playerFearController).AsSingle();
            Container.Bind<InputController>().FromInstance(_inputController).AsSingle();
            Container.Bind<GameFlowController>().FromInstance(_gameFlowController).AsSingle();
            Container.Bind<FearAttractionManager>().FromInstance(_fearAttractionManager).AsSingle();
        }
    }
}