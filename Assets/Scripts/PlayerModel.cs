using System;
using UniRx;
using Zenject;

public class PlayerModel : IInitializable
{
    private float _maxFearLevel = 100f;
    
    private ReactiveProperty<float> _currentFearLevel = new ReactiveProperty<float>();

    private bool _isSitting;

    public bool IsSitting => _isSitting;

    public void SetSitting(bool sitting) => _isSitting = sitting;

    public void Initialize()
    {
        _currentFearLevel.Value = 0;
    }

}