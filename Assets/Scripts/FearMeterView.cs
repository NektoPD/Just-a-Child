using System;
using Clicker;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Math = System.Math;

public class FearMeterView : MonoBehaviour, IInitializable
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private AnimatedText _animatedText;
    [SerializeField] private float _fillBarAnimationDuration = 0.3f;

    private float _maxTextValue;
    private PlayerConfig _playerConfig;

    [Inject]
    public void Construct(PlayerConfig playerConfig)
    {
        _playerConfig = playerConfig;
    }

    [Inject]
    public void Initialize()
    {
        _maxTextValue = _playerConfig.MaxFearValue;
        _animatedText.SetMaxValue(_maxTextValue);
        _animatedText.SetValue(0);
    }

    public void UpdateVisuals(float value)
    {
        var endValue = Mathf.InverseLerp(0, _maxTextValue, value);

        _fillBar.DOFillAmount(endValue, _fillBarAnimationDuration);
        
        _animatedText.SetValue(value);
    }

}
