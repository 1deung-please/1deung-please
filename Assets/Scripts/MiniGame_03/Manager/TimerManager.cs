using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    float time = 30f;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordMiniGamePlay(3);
        }
    }

    bool isGameOver = false;

    void Update()
    {
        if (isGameOver)
            return;

        time -= Time.deltaTime;

        if (time < 0)
            time = 0;

        timerText.text = time.ToString("F1");

        if (time <= 0)
        {
            isGameOver = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RecordMiniGameResult(3, false);
            }

            Debug.Log("시간 종료");
        }
    }
}