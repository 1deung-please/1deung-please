using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Result UI Texts")]
    public TMP_Text TitleText;
    public TMP_Text ResultReasonText;
    public TMP_Text RecordText;
    
    [Header("Game Settings")]
    public float maxGameTime = 20f;
    float currentGameTime;

    public float maxHealth = 100f;
    float currentHealth;

    public int scorePerCorrect = 10;
    int currentMeritScore = 0;

    bool needSeat;
    bool isGameOver;
    bool isGameStarted;

    int correctCount;
    int wrongCount;

    List<float> clickTimestamps = new List<float>(); 
    float maxCPS = 0f;                               

    void Start()
    {
        if (titlePanel != null) titlePanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);

        isGameStarted = false;
        isGameOver = false;
    }

    public void StartGame()
    {
        if (titlePanel != null) titlePanel.SetActive(false);

        currentGameTime = maxGameTime;
        currentHealth = maxHealth;
        currentMeritScore = 0;
        correctCount = 0;
        wrongCount = 0;

        clickTimestamps.Clear();
        maxCPS = 0f;

        isGameOver = false;
        isGameStarted = true;

        UpdateMeritUI();
        SpawnPassenger();
    }

    void Update()
    {
        if (!isGameStarted || isGameOver)
            return;

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

        UpdateCPSData();

        if (currentHealth <= 0)
        {
            GameOver(isSuccess: false);
        }
        else if (currentGameTime <= 0)
        {
            GameOver(isSuccess: true);
        }
    }

    void RegisterClick()
    {
        clickTimestamps.Add(Time.time);
        UpdateCPSData();
    }

    void UpdateCPSData()
    {
        float currentTime = Time.time;
        int current1SecClicks = 0;

        for (int i = clickTimestamps.Count - 1; i >= 0; i--)
        {
            if (currentTime - clickTimestamps[i] <= 1.0f)
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

    void SpawnPassenger()
    {
        int random = Random.Range(0, passengerSprites.Length);
        passengerImage.sprite = passengerSprites[random];

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

    public void GiveSeat()
    {
        if (!isGameStarted || isGameOver) return;

        RegisterClick();

        if (needSeat)
        {
            currentHealth += 15f;
            currentMeritScore += scorePerCorrect;
            correctCount++;
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateMeritUI();
        SpawnPassenger();
    }

    public void IgnoreSeat()
    {
        if (!isGameStarted || isGameOver) return;

        RegisterClick();

        if (!needSeat)
        {
            currentHealth += 15f;
            currentMeritScore += scorePerCorrect;
            correctCount++;
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateMeritUI();
        SpawnPassenger();
    }

    void UpdateMeritUI()
    {
        if (meritText != null)
            meritText.text = $"공덕 {currentMeritScore}";
    }

    void GameOver(bool isSuccess)
    {
        isGameOver = true;

        int finalScore = isSuccess ? currentMeritScore : 0;
        string reason = isSuccess ? "클리어 (정상)" : "게이지0 (오버)";

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            if (TitleText != null)
                TitleText.text = "플레이 기록";

            float survivedTime =
                maxGameTime - Mathf.Max(0, currentGameTime);

            int totalAttempts =
                correctCount + wrongCount;

            float accuracy =
                totalAttempts > 0
                ? ((float)correctCount / totalAttempts) * 100f
                : 0f;

            float avgCPS =
                survivedTime > 0
                ? (float)totalAttempts / survivedTime
                : 0f;

            if (ResultReasonText != null)
                ResultReasonText.text = reason;

            if (RecordText != null)
            {
                RecordText.text =
                    $"공덕 {finalScore} " +
                    $"생존 {survivedTime:F0}s " +
                    $"정답/오답 {correctCount}/{wrongCount} " +
                    $"정확도 {accuracy:F0}% " +
                    $"평균 CPS {avgCPS:F1} " +
                    $"최고 CPS {maxCPS:F0}";
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}