using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager_mg02 : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titlePanel;
    public GameObject resultPanel;

    [Header("In-Game UI")]
    public Image passengerImage;
    public Sprite[] passengerSprites;
    public Slider healthBar;
    public TMP_Text timerText;
    public TMP_Text meritText;
    public TMP_Text currentMeritText;
    public TMP_Text decreaseText;

    [Header("Result UI Texts")]
    public TMP_Text titleText;
    public TMP_Text resultReasonText;
    public TMP_Text recordText;

    [Header("Game Settings")]
    public float maxGameTime = 20f;
    float currentGameTime;
    public float maxHealth = 100f;
    float currentHealth;

    bool needSeat;
    bool isGameOver;
    bool isGameStarted;

    int correctCount;
    int wrongCount;

    List<float> clickTimestamps = new List<float>();
    float maxCPS = 0f;

    void Start()
    {
        if (titlePanel != null)
            titlePanel.SetActive(true);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        isGameStarted = false;
        isGameOver = false;
    }

    public void StartGame()
    {
        if (titlePanel != null)
            titlePanel.SetActive(false);

        currentGameTime = maxGameTime;
        currentHealth = maxHealth;

        correctCount = 0;
        wrongCount = 0;

        clickTimestamps.Clear();
        maxCPS = 0f;
        isGameOver = false;
        isGameStarted = true;

        updateMeritUI();

        spawnPassenger();
    }

    void Update()
    {
        if (!isGameStarted || isGameOver) return;

        currentGameTime -= Time.deltaTime;

        float decreaseSpeed = 10f;

        if (currentGameTime <= 14f && currentGameTime > 10f)
        {
            decreaseSpeed = 15f;
        }
        else if (currentGameTime <= 9f && currentGameTime > 5f)
        {
            decreaseSpeed = 20f;
        }
        else if (currentGameTime <= 4f)
        {
            decreaseSpeed = 25f;
        }

        currentHealth -= decreaseSpeed * Time.deltaTime;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.value = currentHealth / maxHealth;
        timerText.text = Mathf.Ceil(currentGameTime) + "s";

        if (decreaseText != null)
        {
            decreaseText.text = $"-{decreaseSpeed:F0}/s";
        }

        updateCpsData();

        if (currentHealth <= 0)
        {
            gameOver(false);
        }
        else if (currentGameTime <= 0)
        {
            gameOver(true);
        }
    }

    void registerClick()
    {
        clickTimestamps.Add(Time.time);
        updateCpsData();
    }

    void updateCpsData()
    {
        float currentTime = Time.time;
        int current1SecClicks = 0;

        for (int i = clickTimestamps.Count - 1; i >= 0; i--)
        {
            if (currentTime - clickTimestamps[i] <= 1f)
            {
                current1SecClicks++;
            }
            else
            {
                break;
            }
        }

        if (current1SecClicks > maxCPS)
        {
            maxCPS = current1SecClicks;
        }
    }

    void spawnPassenger()
    {
        int random = Random.Range(0, passengerSprites.Length);

        passengerImage.sprite =
            passengerSprites[random];

        switch (random)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                needSeat = true;
                break;

            case 4:
            case 5:
            case 6:
                needSeat = false;
                break;
        }
    }

    public void giveSeat()
    {
        if (!isGameStarted || isGameOver)
            return;

        registerClick();

        if (needSeat)
        {
            currentHealth += 15f;
            correctCount++;
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        updateMeritUI();
        spawnPassenger();
    }

    public void ignoreSeat()
    {
        if (!isGameStarted || isGameOver)
            return;

        registerClick();

        if (!needSeat)
        {
            currentHealth += 15f;
            correctCount++;
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
        }

        currentHealth = Mathf.Clamp( currentHealth, 0, maxHealth);

        updateMeritUI();
        spawnPassenger();
    }

    void updateMeritUI()
    {
        if (meritText != null)
        {
            meritText.text =
                $"정답 {correctCount}";
        }

        if (currentMeritText != null)
        {
            int earnedMerit = correctCount * 20;
            currentMeritText.text = $"공덕 {earnedMerit}";
        }
    }

    void gameOver(bool isSuccess)
    {
        isGameOver = true;

        // 전역 공덕 시스템 전달
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMiniGameComplete(2, correctCount);
        }

        string reason;

        if (isSuccess)
        {
            reason = "시간종료(정상)";
        }
        else
        {
            reason = "게이지0(오버)";
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            if (titleText != null)
                titleText.text = "플레이 기록";

            float survivedTime =
                maxGameTime -
                Mathf.Max(0, currentGameTime);

            int totalAttempts =
                correctCount + wrongCount;

            float accuracy =
                totalAttempts > 0
                ? ((float)correctCount / totalAttempts) * 100f : 0f;

            float avgCps =
                survivedTime > 0
                ? (float)totalAttempts / survivedTime: 0f;

            if (resultReasonText != null)
                resultReasonText.text = reason;

            if (recordText != null)
            {
                int earnedMerit =
                    correctCount * 20;

                recordText.text =
                    $"공덕 {earnedMerit} " +
                    $"생존 {survivedTime:F0}s " +
                    $"정답/오답 {correctCount}/{wrongCount} " +
                    $"정확도 {accuracy:F0}% " +
                    $"평균 CPS {avgCps:F1} " +
                    $"최고 CPS {maxCPS:F0} ";
            }
        }
    }

    public void restartGame()
    {
        SceneLoader.Instance.LoadScene("MiniGame_02");
    }

    public void returnToLobby()
    {
        GameManager.Instance.ReturnToLobby();
    }
}