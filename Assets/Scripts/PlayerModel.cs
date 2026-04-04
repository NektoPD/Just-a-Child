using System;
using UniRx;
using Zenject;

public class PlayerModel : IInitializable
{
    private float _maxFearLevel = 100f;
    private PlayerConfig _playerConfig;
    
    private ReactiveProperty<float> _currentFearLevel = new ReactiveProperty<float>();
    public IReadOnlyReactiveProperty<float> CurrentFearLevel => _currentFearLevel;

    public void Initialize()
    {
        _playerConfig = playerConfig;
        _currentFearLevel.Value = 0;
    }

    public void IncreaseFear()
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value + _playerConfig.FearIncreaseValue, 0,
            _playerConfig.MaxFearValue);
    }

    public void DecreaseFear()
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value - _playerConfig.FearDecreaseValue, 0,
            _playerConfig.MaxFearValue);
    }
    
}