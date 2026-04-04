using System;
using UniRx;
using Zenject;

public class PlayerModel : IInitializable
{
    private float _maxFearLevel = 100f;
    private PlayerConfig _playerConfig;
    
    private ReactiveProperty<float> _currentFearLevel = new ReactiveProperty<float>();

    public PlayerModel(PlayerConfig playerConfig)
    {
        _playerConfig = playerConfig;
    }
    
    public IReadOnlyReactiveProperty<float> CurrentFearLevel => _currentFearLevel;
    public bool IsFearMaxed => _currentFearLevel.Value >= _playerConfig.MaxFearValue;

    public void Initialize()
    {
        _currentFearLevel.Value = 0;
    }

    public void IncreaseFear()
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value + _playerConfig.FearIncreaseValue, 0,
            _playerConfig.MaxFearValue);
    }

    public void IncreaseFearBy(float amount)
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value + amount, 0,
            _playerConfig.MaxFearValue);
    }

    public void DecreaseFear()
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value - _playerConfig.FearDecreaseValue, 0,
            _playerConfig.MaxFearValue);
    }

    public void DecreaseFearBy(float amount)
    {
        _currentFearLevel.Value = Math.Clamp(_currentFearLevel.Value - amount, 0,
            _playerConfig.MaxFearValue);
    }
    
}