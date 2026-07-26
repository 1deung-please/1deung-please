using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    float time = 30f;

    void Start()
    {
        GameManager.Instance.RecordMiniGamePlay(3);
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

            GameManager.Instance.RecordMiniGameResult(3, false);
            GameManager.Instance.OnMiniGameComplete(3, 0);

            Debug.Log("시간 종료");
        }
    }
}