using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;

    public void UpdateTimer(float remainingSeconds)
    {
        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
