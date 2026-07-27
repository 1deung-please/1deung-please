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
    public GameObject readyPanel;      // ����� ��� �г�
    public GameObject countdownPanel;  // 3,2,1 �г�
    public GameObject resultPanel;     // ��� �г�
    public TMP_Text targetText;        // "192�� �̻� �����⸦ �ݰŶ�!"
    public TMP_Text countdownText;     // 3,2,1
    public TMP_Text timerText;         // TIME 8.46
    public Slider timerBar;            // Ÿ�̸� ��
    public TMP_Text collectCountText;  // ���� ��� ���� ����
    public TMP_Text resultReasonText;  // ����/����
    public TMP_Text resultRecordText;  // ��ǥ/����/����
    public TMP_Text meritText;         // ����

    private MiniGame01Phase currentPhase;
    private int targetCount;
    private int currentCount;
    private float remainingTime;

    void Start()
    {
        currentPhase = MiniGame01Phase.Ready;
        targetCount = Random.Range(minTarget, maxTarget + 1);

        if (targetText != null)
            targetText.text = $"��... {targetCount}�� �̻� �����⸦ �ݰŶ�!";

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

        // 플레이 기록
        GameManager.Instance.RecordMiniGamePlay(1);

        ShowPanel(null); // ���� ȭ���� ���� �г� ���� �׻� ���̴� ����̶� ����
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
            collectCountText.text = $"{currentCount}��";
    }

    void EndGame(bool naturalEnd)
    {
        currentPhase = MiniGame01Phase.Result;

        bool isSuccess = currentCount >= targetCount;

        // 성공/실패 기록
        GameManager.Instance.RecordMiniGameResult(1, isSuccess);

        int merit = isSuccess
            ? currentCount + successBonus
            : Mathf.RoundToInt(currentCount * failPenaltyRate);

        ShowPanel(resultPanel);

        if (resultReasonText != null)
            resultReasonText.text = isSuccess ? "SUCCESS" : "FAIL";

        if (resultRecordText != null)
            resultRecordText.text =
                $"������� �ֿ��� �� ������ ��: {targetCount}��\n" +
                $"���ΰ��� �ֿ� ������ ��: {currentCount}��";

        if (meritText != null)
            meritText.text = $"ȹ�� ����: {merit}P";

        GameManager.Instance.OnMiniGameComplete(1, merit);
    }

    void ShowPanel(GameObject target)
    {
        if (readyPanel != null) readyPanel.SetActive(target == readyPanel);
        if (countdownPanel != null) countdownPanel.SetActive(target == countdownPanel);
        if (resultPanel != null) resultPanel.SetActive(target == resultPanel);
    }

    // ��� ȭ�� ��ư��
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