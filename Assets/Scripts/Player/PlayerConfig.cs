using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig/Config")]
public class PlayerConfig : ScriptableObject
{
    [field: SerializeField] public float MaxFearValue { get; private set; }
    [field: SerializeField] public float FearDecreaseValue { get; private set; }
    [field: SerializeField] public float FearIncreaseValue { get; private set; }
    [field: SerializeField] public float FearIncreaseInterval { get; private set; }
    [field: SerializeField] public float FearDecreaseCooldown { get; private set; }
}