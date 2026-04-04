using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Furniture
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class LightSource : MonoBehaviour
    {
        [SerializeField] private Light2D _light2D;

        private CircleCollider2D _circleCollider2D;
        private PlayerFearController _playerFearController;
        
        [Inject]
        public void Construct(PlayerFearController playerFearController)
        {
            _playerFearController = playerFearController;
        }

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            _circleCollider2D.radius = _light2D.pointLightOuterRadius + _light2D.pointLightInnerRadius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(_playerFearController == null)
                return;
            
            
        }
    }
}