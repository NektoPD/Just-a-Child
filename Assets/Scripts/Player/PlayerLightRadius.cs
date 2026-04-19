using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Player
{
    [RequireComponent(typeof(Light2D))]
    public class PlayerLightRadius : MonoBehaviour
    {
        [SerializeField] private float _innerRadius = 1f;
        [SerializeField] private float _outerRadius = 4f;
        [SerializeField] private float _intensity = 0.8f;

        private Light2D _light2D;

        private void Awake()
        {
            _light2D = GetComponent<Light2D>();
            _light2D.lightType = Light2D.LightType.Point;
            _light2D.pointLightInnerRadius = _innerRadius;
            _light2D.pointLightOuterRadius = _outerRadius;
            _light2D.intensity = _intensity;
        }
    }
}
