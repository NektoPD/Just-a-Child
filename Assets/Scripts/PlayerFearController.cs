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
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private InputController _inputController;
    
    private float _currentFearLevel;
    private PlayerModel _playerModel;
    private Coroutine _fearIncreaseCoroutine;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(PlayerModel fearModel)
    {
        _playerModel = fearModel;
    }

    private void Start()
    {
        _playerModel.Initialize(_playerConfig);
        _playerModel.CurrentFearLevel.Subscribe(_fearMeterView.UpdateVisuals).AddTo(_disposables);
        
        _fearMeterView.UpdateVisuals(_playerModel.CurrentFearLevel.Value);
        _fearMeterView.Initialize(_playerConfig.MaxFearValue);
        
        _fearIncreaseCoroutine = StartCoroutine(FearIncreaseCoroutine());

        _inputController.OnInteractionPerformed.Subscribe(_ => _playerModel.DecreaseFear()).AddTo(_disposables);
    }

    private IEnumerator FearIncreaseCoroutine()
    {
        while (true)
        {
            _playerModel.IncreaseFear();
            yield return new WaitForSeconds(_playerConfig.FearIncreaseInterval);
        }
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}
