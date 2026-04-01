using DefaultNamespace;
using UnityEngine;

public class PlayerFearController : MonoBehaviour
{
    [SerializeField] private FearMeter _fearMeter;
    
    private float _currentFearLevel;
    
    public FearConfig FearConfig { get; private set; }

    public void Initialize(FearConfig fearConfig)
    {
        FearConfig = fearConfig;
    }

    public void IncreaseLevel()
    {}
}
