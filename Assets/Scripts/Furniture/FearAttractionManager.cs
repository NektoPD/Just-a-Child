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

        public void StartSpawning()
        {
            StopSpawning();
            _spawningCoroutine = StartCoroutine(SpawningCoroutine());
        }

        public void StopSpawning()
        {
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

        private IEnumerator SpawningCoroutine()
        {
            while (true)
            {
                float delay = Random.Range(_minActivationInterval, _maxActivationInterval);
                yield return new WaitForSeconds(delay);

                FearAttraction candidate = GetRandomInactive();
                if (candidate != null)
                    candidate.Activate();
            }
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
