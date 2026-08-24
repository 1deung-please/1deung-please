using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;
    private float time = 60f;
    private bool timerEnded = false;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordMiniGamePlay(3);
        }

        //소수점 첫째 자리까지 표시
        timerText.text = time.ToString("F1");
    }

    void Update()
    {
        if (timerEnded)
            return;

        time -= Time.deltaTime;

        if (time < 0)
            time = 0;

        timerText.text = time.ToString("F1");

        //시간이 끝나면
        if (time <= 0)
        {
            timerEnded = true;

            if (Game3Manager.Instance != null)
                Game3Manager.Instance.GameFail();
        }
    }
}