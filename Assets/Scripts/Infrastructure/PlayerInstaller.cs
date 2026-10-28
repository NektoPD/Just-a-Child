using Cinemachine;
using Player;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Character _playerPrefab;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    public override void InstallBindings()
    {
        Character player = Container.InstantiatePrefabForComponent<Character>(
            _playerPrefab,
            _playerSpawnPoint.position,
            _playerSpawnPoint.rotation,
            null);

        Container.Bind<CinemachineVirtualCamera>().FromInstance(_virtualCamera).AsSingle();

        Container.BindInterfacesTo<CameraFollow>().AsSingle().NonLazy();

        Container.Bind<Character>()
            .FromInstance(player)
            .AsSingle()
            .NonLazy();
    }
}
