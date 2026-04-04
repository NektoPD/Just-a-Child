using System;
using Clicker;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;
using Math = System.Math;

public class FearMeterView : MonoBehaviour
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private AnimatedText _animatedText;
    [SerializeField] private float _fillBarAnimationDuration = 0.3f;

    private float _maxTextValue;

    public void Initialize(float maxTextValue)
    {
        _maxTextValue = maxTextValue;
        _animatedText.SetMaxValue(_maxTextValue);
    }

    public void UpdateVisuals(float value)
    {
        var endValue = Mathf.InverseLerp(0, _maxTextValue, value);

        _fillBar.DOFillAmount(endValue, _fillBarAnimationDuration);
        
        _animatedText.SetValue(value);
    }

}
