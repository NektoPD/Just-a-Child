using System;
using System.Collections;
using Player;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class PlayerFearController : MonoBehaviour
{
    [SerializeField] private FearMeterView _fearMeterView;
    [SerializeField] private InputController _inputController;

    private PlayerConfig _playerConfig;
    private float _currentFearLevel;
    private PlayerModel _playerModel;
    private Coroutine _fearIncreaseCoroutine;
    private Coroutine _fearDecreaseCoroutine;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(PlayerModel fearModel, PlayerConfig playerConfig)
    {
        _playerModel = fearModel;
        _playerConfig = playerConfig;
    }

    private void Start()
    {
        _playerModel.CurrentFearLevel.Subscribe(_fearMeterView.UpdateVisuals).AddTo(_disposables);

        _fearMeterView.UpdateVisuals(_playerModel.CurrentFearLevel.Value);
        
        _inputController.OnInteractionPerformed.Subscribe(_ => _playerModel.DecreaseFear()).AddTo(_disposables);
    }

    public void StartIncreasingFear()
    {
        _fearIncreaseCoroutine = StartCoroutine(FearIncreaseCoroutine());
    }

    public void StopIncreasingFear()
    {
        if(_fearIncreaseCoroutine == null)
            return;
        
        StopCoroutine(_fearIncreaseCoroutine);
        _fearIncreaseCoroutine = null;
    }

    public void StartDecreaseFearWithCooldown()
    {
        _fearDecreaseCoroutine = StartCoroutine(FearDecreaseCoroutine());
    }

    public void StopDecreaseFearWithCooldown()
    {
        if(_fearDecreaseCoroutine == null)
            return;
        
        StopCoroutine(_fearDecreaseCoroutine);
        _fearDecreaseCoroutine = null;
    }
    
    private IEnumerator FearIncreaseCoroutine()
    {
        WaitForSeconds interval = new WaitForSeconds(_playerConfig.FearIncreaseInterval);

        yield return interval;
        
        while (true)
        {
            _playerModel.IncreaseFear();
            yield return interval;
        }
    }

    private IEnumerator FearDecreaseCoroutine()
    {
        while (true)
        {
            _playerModel.DecreaseFear();
            yield return new WaitForSeconds(_playerConfig.FearDecreaseCooldown);
        }
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}