using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Furniture
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class LightSource : MonoBehaviour
    {
        [SerializeField] private Light2D _light2D;

        private CircleCollider2D _circleCollider2D;

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
            
        }
    }
}