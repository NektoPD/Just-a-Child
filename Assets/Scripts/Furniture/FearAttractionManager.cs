using System.Collections;
using UnityEngine;
using Zenject;

namespace Furniture
{
    public class FearAttractionManager : MonoBehaviour
    {
        [SerializeField] private FearAttraction[] _attractions;

        [Header("Base Intervals")]
        [SerializeField] private float _minActivationInterval = 10f;
        [SerializeField] private float _maxActivationInterval = 25f;

        [Header("Escalation")]
        [SerializeField] private float _intervalDecreasePerSecond = 0.05f;
        [SerializeField] private float _minIntervalFloor = 3f;
        [SerializeField] private float _secondsPerExtraSlot = 60f;
        [SerializeField] private int _maxSimultaneous = 4;

        private Coroutine _spawningCoroutine;
        private bool _isRunning;
        private float _elapsedTime;
        private int _allowedActive = 1;

        public void StartSpawning()
        {
            StopSpawning();
            _isRunning = true;
            _elapsedTime = 0f;
            _allowedActive = 1;

            foreach (var attraction in _attractions)
                attraction.OnInvestigated += OnAttractionInvestigated;

            _spawningCoroutine = StartCoroutine(SpawningLoop());
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
            TryActivateOne();
        }

        private IEnumerator SpawningLoop()
        {
            yield return new WaitForSeconds(GetCurrentInterval());

            while (_isRunning)
            {
                _elapsedTime += GetCurrentInterval();
                _allowedActive = Mathf.Min(1 + Mathf.FloorToInt(_elapsedTime / _secondsPerExtraSlot), _maxSimultaneous);

                if (GetActiveCount() < _allowedActive)
                    TryActivateOne();

                yield return new WaitForSeconds(GetCurrentInterval());
            }

            _spawningCoroutine = null;
        }

        private float GetCurrentInterval()
        {
            float reduction = _elapsedTime * _intervalDecreasePerSecond;
            float min = Mathf.Max(_minActivationInterval - reduction, _minIntervalFloor);
            float max = Mathf.Max(_maxActivationInterval - reduction, _minIntervalFloor);
            return Random.Range(min, max);
        }

        private void TryActivateOne()
        {
            FearAttraction candidate = GetRandomInactive();
            if (candidate != null)
                candidate.Activate();
        }

        private int GetActiveCount()
        {
            int count = 0;
            foreach (var a in _attractions)
            {
                if (a.IsActive) count++;
            }
            return count;
        }

        public FearAttraction GetRandomActive()
        {
            int count = 0;
            foreach (var a in _attractions)
            {
                if (a.IsActive) count++;
            }
            if (count == 0) return null;
            int pick = Random.Range(0, count);
            int idx = 0;
            foreach (var a in _attractions)
            {
                if (a.IsActive)
                {
                    if (idx == pick) return a;
                    idx++;
                }
            }
            return null;
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
