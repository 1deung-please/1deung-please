using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject successImage;
    public GameObject failImage;
    public TMP_Text resultReasonText;
    public TMP_Text recordText;

    [Header("Result Buttons")]
    public Button restartButton;

    [Header("Animation")]
    public Image flashPanel;
    public Image startImage;

    Coroutine blinkCoroutine;
    Coroutine idleCoroutine;
    Coroutine punchScaleCoroutine; // PunchScale 중첩 제어용 변수

    private readonly Vector3 baseScale = Vector3.one; // NPC의 기준 크기 고정

    [Header("Sound")]
    public AudioSource audioSource;

    public AudioClip buttonSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;

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
        if (successImage != null)
            successImage.SetActive(false);

        if (failImage != null)
            failImage.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        isGameStarted = false;
        isGameOver = false;

        if (startImage != null)
        {
            startImage.gameObject.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkImage());
        }

        if (flashPanel != null)
        {
            Color color = flashPanel.color;
            color.a = 0f;
            flashPanel.color = color;
            flashPanel.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMiniGameStart();
            GameManager.Instance.RecordMiniGamePlay(2);
        }

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

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (startImage != null)
        {
            Color color = startImage.color;
            color.a = 0f;
            startImage.color = color;
            startImage.gameObject.SetActive(false);
        }

        updateMeritUI();

        StartCoroutine(FlashRoutine());

        spawnPassenger();
    }

    void Update()
    {
        if (!isGameStarted || isGameOver) return;

        currentGameTime -= Time.deltaTime;

        float decreaseSpeed;

        if (currentGameTime > 15f)
        {
            decreaseSpeed = 10f;
        }
        else if (currentGameTime > 10f)
        {
            decreaseSpeed = 15f;
        }
        else if (currentGameTime > 5f)
        {
            decreaseSpeed = 20f;
        }
        else
        {
            decreaseSpeed = 25f;
        }

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

        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }

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
        if (passengerImage == null)
            return;

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

        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }

        idleCoroutine = StartCoroutine(PassengerIdleRoutine());
    }

    public void giveSeat()
    {
        if (!isGameStarted || isGameOver)
            return;

        registerClick();
        audioSource.PlayOneShot(buttonSound);

        if (needSeat)
        {
            currentHealth += 15f;
            correctCount++;
            audioSource.PlayOneShot(correctSound);
            TriggerPunchScale();
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
            audioSource.PlayOneShot(wrongSound);
            StartCoroutine(ShakeRoutine(passengerImage.GetComponent<RectTransform>()));
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        StartCoroutine(HealthBarRoutine(currentHealth / maxHealth));
        updateMeritUI();
        spawnPassenger();
    }

    public void ignoreSeat()
    {
        if (!isGameStarted || isGameOver)
            return;

        registerClick();
        audioSource.PlayOneShot(buttonSound);

        if (!needSeat)
        {
            currentHealth += 15f;
            correctCount++;
            audioSource.PlayOneShot(correctSound);
            TriggerPunchScale();
        }
        else
        {
            currentHealth -= 30f;
            wrongCount++;
            audioSource.PlayOneShot(wrongSound);
            StartCoroutine(
                ShakeRoutine(
                    passengerImage.GetComponent<RectTransform>()
                )
            );
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        StartCoroutine(HealthBarRoutine(currentHealth / maxHealth));
        updateMeritUI();
        spawnPassenger();
    }

    // 정답 시 연출 중첩 방지 및 코루틴 실행
    void TriggerPunchScale()
    {
        if (passengerImage == null) return;

        if (punchScaleCoroutine != null)
        {
            StopCoroutine(punchScaleCoroutine);
            passengerImage.transform.localScale = baseScale; // 즉시 원본 스케일로 복구
        }

        punchScaleCoroutine = StartCoroutine(PunchScaleRoutine(passengerImage.transform));
    }

    void updateMeritUI()
    {
        if (meritText != null)
        {
            meritText.text = $"정답 {correctCount}";
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
            GameManager.Instance.CompleteMiniGame2(correctCount);
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.OnMiniGameResult(MiniGameKind.DontMove, isSuccess);
        }

        if (resultPanel != null)
        {
            if (successImage != null)
                successImage.SetActive(isSuccess);

            if (failImage != null)
                failImage.SetActive(!isSuccess);

            if (resultReasonText != null)
            {
                resultReasonText.text =
                    $"맞힌 문제: {correctCount}문제 " +
                    $"틀린 문제: {wrongCount}문제\n" +
                    $"도믿걸 남은 체력: {currentHealth:F0} / {maxHealth:F0}";
            }

            if (recordText != null)
            {
                int earnedMerit = correctCount * 20;
                recordText.text = earnedMerit.ToString();
            }
            StartCoroutine(ShowPanelDelay());
        }

        bool willAutoReturn = GameManager.Instance != null &&
                              GameManager.Instance.IsPendingEndingTransition();

        if (restartButton != null)
            restartButton.gameObject.SetActive(!willAutoReturn);
    }

    IEnumerator ShowPanelDelay()
    {
        yield return new WaitForSeconds(1.0f); 

        resultPanel.SetActive(true); 
        StartCoroutine(ResultPanelRoutine()); 
    }

    public void restartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MiniGame_02");
    }

    public void returnToLobby()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToLobby();
        }
        else
        {
            Debug.LogError("GameManager.Instance NULL");
        }
    }

    IEnumerator AutoReturnToLobbyAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ReturnToLobby();
    }

    IEnumerator BlinkImage()
    {
        if (startImage == null)
            yield break;

        Color originalColor = startImage.color;

        while (!isGameStarted)
        {
            float alpha = Mathf.PingPong(Time.time * 1.5f, 1f);

            Color color = startImage.color;
            color.a = alpha;
            startImage.color = color;

            yield return null;
        }
        originalColor.a = 1f;
        startImage.color = originalColor;
    }

    IEnumerator FlashRoutine()
    {
        if (flashPanel == null)
            yield break;

        flashPanel.gameObject.SetActive(true);

        Color color = flashPanel.color;
        color.a = 1f;
        flashPanel.color = color;

        float time = 0f;
        float duration = 0.3f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);

            color.a = alpha;
            flashPanel.color = color;

            yield return null;
        }

        color.a = 0f;
        flashPanel.color = color;
        flashPanel.gameObject.SetActive(false);
    }

    IEnumerator PassengerIdleRoutine()
    {
        if (passengerImage == null)
            yield break;

        RectTransform passenger = passengerImage.GetComponent<RectTransform>();

        if (passenger == null)
            yield break;

        Vector2 originalPosition = passenger.anchoredPosition;

        while (isGameStarted && !isGameOver)
        {
            float offset = Mathf.Sin(Time.time * 2f) * 3f;
            passenger.anchoredPosition = originalPosition + new Vector2(0f, offset);
            yield return null;
        }

        passenger.anchoredPosition = originalPosition;
    }

    // 수정된 Punch Scale (고정된 baseScale 기준 연출)
    IEnumerator PunchScaleRoutine(Transform target)
    {
        if (target == null) yield break;

        Vector3 punchScale = baseScale * 1.15f;

        float duration = 0.2f;
        float time = 0f;

        while (time < duration / 2f)
        {
            time += Time.deltaTime;
            float t = time / (duration / 2f);

            target.localScale = Vector3.Lerp(baseScale, punchScale, t);
            yield return null;
        }

        time = 0f;

        while (time < duration / 2f)
        {
            time += Time.deltaTime;
            float t = time / (duration / 2f);

            target.localScale = Vector3.Lerp(punchScale, baseScale, t);
            yield return null;
        }

        target.localScale = baseScale;
        punchScaleCoroutine = null;
    }

    IEnumerator ShakeRoutine(RectTransform target)
    {
        if (target == null)
            yield break;

        Vector2 originalPosition = target.anchoredPosition;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float x = Mathf.Sin(time * 50f) * 10f;

            target.anchoredPosition = originalPosition + new Vector2(x, 0f);

            yield return null;
        }

        target.anchoredPosition = originalPosition;
    }

    IEnumerator HealthBarRoutine(float targetValue)
    {
        if (healthBar == null)
            yield break;

        float startValue = healthBar.value;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            healthBar.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        healthBar.value = targetValue;
    }

    IEnumerator ResultPanelRoutine()
    {
        if (resultPanel == null)
            yield break;

        Transform panel = resultPanel.transform;

        panel.localScale = Vector3.zero;

        float time = 0f;
        float duration = 0.4f;

        while (time < duration)
        {
            time += Time.deltaTime;
            
            float t = Mathf.Clamp01(time / duration);

            float overshoot = 1.70158f;
            float backT = t - 1f;
            t = backT * backT * ((overshoot + 1f) * backT + overshoot) + 1f;

            panel.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return null;
        }

        panel.localScale = Vector3.one;
    }
}