using System.Collections;
using UnityEngine;
using Zenject;

public class RandomEncounterManager : MonoBehaviour
{
    [SerializeField] private GhostCreature _ghostPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _minSpawnInterval = 20f;
    [SerializeField] private float _maxSpawnInterval = 45f;
    [SerializeField] private SpeechBubbleView _playerSpeechBubble;
    [SerializeField, TextArea(2, 5)] private string _ghostReactionText = "What was that?!";
    [SerializeField] private AudioSource _playerAudioSource;
    [SerializeField] private AudioClip _ghostReactionSound;

    private GhostCreature _currentGhost;
    private Coroutine _spawnCoroutine;
    private bool _isRunning;
    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }

    public void StartEncounters()
    {
        _isRunning = true;
        _spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopEncounters()
    {
        _isRunning = false;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (_isRunning)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnInterval, _maxSpawnInterval));
            if (!_isRunning) yield break;

            Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            SpawnGhost(point.position);
        }
    }

    private void SpawnGhost(Vector3 position)
    {
        if (_currentGhost == null)
        {
            _currentGhost = _container.InstantiatePrefabForComponent<GhostCreature>(_ghostPrefab);
            _currentGhost.SetPlayerTransform(_playerTransform);
            _currentGhost.OnFled += OnGhostFled;
        }

        _currentGhost.Spawn(position);
    }
<<<<<<< Updated upstream
}
=======

    private void OnGhostFled()
    {
        if (_playerAudioSource != null && _ghostReactionSound != null)
            _playerAudioSource.PlayOneShot(_ghostReactionSound);

        if (_playerSpeechBubble != null && !string.IsNullOrEmpty(_ghostReactionText))
            _playerSpeechBubble.ShowText(_ghostReactionText, null);
    }

    private void OnDestroy()
    {
        if (_currentGhost != null)
            _currentGhost.OnFled -= OnGhostFled;
    }
}
>>>>>>> Stashed changes
