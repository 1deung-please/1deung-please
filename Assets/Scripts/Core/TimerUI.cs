using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public TMP_Text timerText;
    
    void Update()
    {
        if (GameManager.Instance == null) return;

        float time = GameManager.Instance.gameData.globalTimeRemaining;

        int minute = Mathf.FloorToInt(time / 60);
        int second = Mathf.FloorToInt(time % 60);

        timerText.text = $"{minute:00}:{second:00}";

        if (time <= 30)
        {
            float alpha = Mathf.PingPong(Time.time * 3f, 1f);

            Color c = Color.red;
            c.a = alpha;

            timerText.color = c;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}