using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    private float time = 60f;
    private bool timerEnded = false;

    void Update()
    {
        if (timerEnded)
            return;

        time -= Time.deltaTime;

        if (time < 0)
            time = 0;

        timerText.text = time.ToString("F1");

        if (time <= 0)
        {
            timerEnded = true;

            Debug.Log("시간 종료");

            if (Game3Manager.Instance != null)
                Game3Manager.Instance.GameFail();
        }
    }
}