using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MiniGame01Phase { Ready, Countdown, Playing, Result }

public class MiniGame01Controller : MonoBehaviour
{
    [Header("Settings")]
    public int minTarget = 150;
    public int maxTarget = 200;
    public float timeLimit = 10f;
    public int successBonus = 50;
    public float failPenaltyRate = 0.5f;
    public int countdownSeconds = 3;

    [Header("UI")]
    public GameObject readyPanel;      // 조상신 대사 패널
    public GameObject countdownPanel;  // 3,2,1 패널
    public GameObject resultPanel;     // 결과 패널
    public TMP_Text targetText;        // "192개 이상 쓰레기를 줍거라!"
    public TMP_Text countdownText;     // 3,2,1
    public TMP_Text timerText;         // TIME 8.46
    public Slider timerBar;            // 타이머 바
    public TMP_Text collectCountText;  // 좌측 상단 수집 개수
    public TMP_Text resultReasonText;  // 성공/실패
    public TMP_Text resultRecordText;  // 목표/수집/공덕
    public TMP_Text meritText;         // 공덕

    private MiniGame01Phase currentPhase;
    private int targetCount;
    private int currentCount;
    private float remainingTime;

    void Start()
    {
        currentPhase = MiniGame01Phase.Ready;
        targetCount = Random.Range(minTarget, maxTarget + 1);

        if (targetText != null)
            targetText.text = $"흠... {targetCount}개 이상 쓰레기를 줍거라!";

        ShowPanel(readyPanel);
    }

    void Update()
    {
        switch (currentPhase)
        {
            case MiniGame01Phase.Ready:
                if (Input.GetMouseButtonDown(0))
                    StartCoroutine(CountdownRoutine());
                break;

            case MiniGame01Phase.Playing:
                UpdatePlaying();
                break;
        }
    }

    IEnumerator CountdownRoutine()
    {
        currentPhase = MiniGame01Phase.Countdown;
        ShowPanel(countdownPanel);

        for (int i = countdownSeconds; i > 0; i--)
        {
            if (countdownText != null) countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        StartPlaying();
    }

    void StartPlaying()
    {
        currentPhase = MiniGame01Phase.Playing;
        currentCount = 0;
        remainingTime = timeLimit;

        ShowPanel(null); // 게임 화면은 별도 패널 없이 항상 보이는 배경이라 가정
        UpdateCollectUI();
    }

    void UpdatePlaying()
    {
        remainingTime -= Time.deltaTime;

        if (timerText != null) timerText.text = remainingTime.ToString("F2");
        if (timerBar != null) timerBar.value = remainingTime / timeLimit;

        if (Input.GetMouseButtonDown(0))
        {
            currentCount++;
            UpdateCollectUI();
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false);
        }
    }

    void UpdateCollectUI()
    {
        if (collectCountText != null)
            collectCountText.text = $"{currentCount}개";
    }

    void EndGame(bool naturalEnd)
    {
        currentPhase = MiniGame01Phase.Result;

        bool isSuccess = currentCount >= targetCount;
        int merit = isSuccess
            ? currentCount + successBonus
            : Mathf.RoundToInt(currentCount * failPenaltyRate);

        ShowPanel(resultPanel);

        if (resultReasonText != null)
            resultReasonText.text = isSuccess ? "SUCCESS" : "FAIL";

        if (resultRecordText != null)
            resultRecordText.text =
                $"조상신이 주우라고 한 쓰레기 수: {targetCount}개\n" +
                $"주인공이 주운 쓰레기 수: {currentCount}개";

        if (meritText != null)
            meritText.text = $"획득 공덕: {merit}P";

        GameManager.Instance.OnMiniGameComplete(1, merit);
    }

    void ShowPanel(GameObject target)
    {
        if (readyPanel != null) readyPanel.SetActive(target == readyPanel);
        if (countdownPanel != null) countdownPanel.SetActive(target == countdownPanel);
        if (resultPanel != null) resultPanel.SetActive(target == resultPanel);
    }

    // 결과 화면 버튼용
    public void OnClickRetry()
    {
        SceneLoader.Instance.LoadScene("MiniGame_01");
    }

    public void OnClickReturnToLobby()
    {
        GameManager.Instance.ReturnToLobby();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), $"Phase: {currentPhase}");
        GUI.Label(new Rect(10, 40, 300, 30), $"Target: {targetCount}");
        GUI.Label(new Rect(10, 70, 300, 30), $"Count: {currentCount}");
        GUI.Label(new Rect(10, 100, 300, 30), $"Time: {remainingTime:F2}");
    }
}