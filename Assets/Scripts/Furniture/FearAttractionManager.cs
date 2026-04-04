using System.Collections;
using UnityEngine;
using Zenject;

namespace Furniture
{
    public class FearAttractionManager : MonoBehaviour
    {
        [SerializeField] private FearAttraction[] _attractions;
        [SerializeField] private float _minActivationInterval = 10f;
        [SerializeField] private float _maxActivationInterval = 25f;

        private Coroutine _spawningCoroutine;
        private bool _isRunning;

        public void StartSpawning()
        {
            StopSpawning();
            _isRunning = true;

            foreach (var attraction in _attractions)
                attraction.OnInvestigated += OnAttractionInvestigated;

            _spawningCoroutine = StartCoroutine(ActivateNextAfterDelay());
        }

        public void StopSpawning()
        {
            _isRunning = false;

            foreach (var attraction in _attractions)
                attraction.OnInvestigated -= OnAttractionInvestigated;

            if (_spawningCoroutine != null)
            {
                StopCoroutine(_spawningCoroutine);
                _spawningCoroutine = null;
            }
        }

        public void DeactivateAll()
        {
            foreach (var attraction in _attractions)
            {
                if (attraction.IsActive)
                    attraction.Deactivate();
            }
        }

        private void OnAttractionInvestigated()
        {
            if (!_isRunning) return;
            _spawningCoroutine = StartCoroutine(ActivateNextAfterDelay());
        }

        private IEnumerator ActivateNextAfterDelay()
        {
            float delay = Random.Range(_minActivationInterval, _maxActivationInterval);
            yield return new WaitForSeconds(delay);

            FearAttraction candidate = GetRandomInactive();
            if (candidate != null)
                candidate.Activate();

            _spawningCoroutine = null;
        }

        private FearAttraction GetRandomInactive()
        {
            int count = 0;
            foreach (var a in _attractions)
            {
                if (!a.IsActive) count++;
            }

            if (count == 0) return null;

            int pick = Random.Range(0, count);
            int idx = 0;
            foreach (var a in _attractions)
            {
                if (!a.IsActive)
                {
                    if (idx == pick) return a;
                    idx++;
                }
            }
            return null;
        }
    }
}
