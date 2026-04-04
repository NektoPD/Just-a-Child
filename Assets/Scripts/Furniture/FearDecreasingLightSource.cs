using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace Furniture
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class FearDecreasingLightSource : MonoBehaviour
    {
        [SerializeField] private Light2D _light2D;
        [SerializeField] private float _useTimer;

        [SerializeField] private float _innerLightStartValue;
        [SerializeField] private float _outerLightStartValue;

        [SerializeField] private float _innerLightEndValue;
        [SerializeField] private float _outerLightEndValue;

        [SerializeField] private float _lightEnableDuration;

        [SerializeField] private float _minIntensity = 0.5f;
        [SerializeField] private float _maxIntensity = 1.5f;
        [SerializeField] private float _flickerSpeed = 0.1f;
        
        [SerializeField] private float _flickerDuration = 2f;
        [SerializeField] private float _finalDisableDelay = 0.5f;

        [SerializeField] private float _lightDuration = 3f;

        private CircleCollider2D _circleCollider2D;
        private PlayerFearController _playerFearController;

        [SerializeField] private bool _isAvailable;
        private Coroutine _flickerCoroutine;
        private Tween _innerRadiusTween;
        private Tween _outerRadiusTween;
        private bool _isLightEnabled;

        [Inject]
        public void Construct(PlayerFearController playerFearController)
        {
            _playerFearController = playerFearController;
        }

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();

            _light2D.enabled = false;
            _light2D.pointLightInnerRadius = _innerLightStartValue;
            _light2D.pointLightOuterRadius = _outerLightStartValue;
            _isLightEnabled = false;
        }

        private void OnDestroy()
        {
            _innerRadiusTween?.Kill();
            _outerRadiusTween?.Kill();
            if (_flickerCoroutine != null)
                StopCoroutine(_flickerCoroutine);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_playerFearController == null || !_isAvailable)
                return;

            EnableLight();
            _isLightEnabled = true;

            _playerFearController.StopIncreasingFear();
            _playerFearController.StartDecreaseFearWithCooldown();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_playerFearController == null)
                return;

            if (_isLightEnabled)
            {
                Debug.Log("exit");
                
                _playerFearController.StartIncreasingFear();
                _playerFearController.StopDecreaseFearWithCooldown();
                
                StartDisableLightSequence();
            }
        }

        public void EnableLight()
        {
            if (_flickerCoroutine != null)
                StopCoroutine(_flickerCoroutine);

            _light2D.intensity = 1f;
            _light2D.enabled = true;

            _innerRadiusTween = DOTween.To(() => _light2D.pointLightInnerRadius, x => _light2D.pointLightInnerRadius = x,
                _innerLightEndValue, _lightEnableDuration);
            
            _outerRadiusTween = DOTween.To(() => _light2D.pointLightOuterRadius, x => _light2D.pointLightOuterRadius = x,
                _outerLightEndValue, _lightEnableDuration);
            
            Invoke(nameof(StartDisableLightSequence), _lightDuration);
        }
        
        public void StartDisableLightSequence()
        {
            if (_flickerCoroutine != null)
                StopCoroutine(_flickerCoroutine);
            
            _flickerCoroutine = StartCoroutine(DisableLightSequence());
        }

        private IEnumerator DisableLightSequence()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < _flickerDuration)
            {
                _light2D.intensity = Random.Range(_minIntensity, _maxIntensity);
                yield return new WaitForSeconds(_flickerSpeed);
                elapsedTime += _flickerSpeed;
            }

            yield return new WaitForSeconds(_finalDisableDelay);

            _light2D.enabled = false;

            _light2D.pointLightInnerRadius = _innerLightStartValue;
            _light2D.pointLightOuterRadius = _outerLightStartValue;
            
            _flickerCoroutine = null;
            _isLightEnabled = false;

            _isAvailable = false;
        }
    }
}