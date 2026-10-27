using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Character _playerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<Character>()
            .FromComponentInNewPrefab(_playerPrefab)
            .UnderTransform(_playerSpawnPoint)
            .AsSingle()
            .NonLazy();
    }
}
