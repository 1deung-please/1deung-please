using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    float time = 30f;

    void Update()
    {
        time -= Time.deltaTime;

        if(time < 0)
            time = 0;

        timerText.text = time.ToString("F1");

        if(time <= 0)
        {
            Debug.Log("시간 종료");
        }
    }
}