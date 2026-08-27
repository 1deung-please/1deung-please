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
        }

        if (GameManager.Instance != null)
        {
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

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

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

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value =
                currentHealth / maxHealth;
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
            StartCoroutine(
                PunchScaleRoutine(
                    passengerImage.transform
                )
            );
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
            StartCoroutine(
                PunchScaleRoutine(
                    passengerImage.transform
                )
            );
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

        currentHealth = Mathf.Clamp( currentHealth, 0, maxHealth);
        StartCoroutine(
            HealthBarRoutine(
                currentHealth / maxHealth
            )
        );
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

        // 전역 공덕 시스템 전달, 결과창에서는 전역 타이머 정지
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
            GameManager.Instance.CompleteMiniGame2(correctCount);
            if (AchievementManager.Instance != null) 
                AchievementManager.Instance.OnMiniGameResult(MiniGameKind.DontMove, isSuccess);
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
            StartCoroutine(ResultPanelRoutine());

            if (successImage != null)
                successImage.SetActive(isSuccess);

            if (failImage != null)
                failImage.SetActive(!isSuccess);

            /*float survivedTime =
                maxGameTime -
                Mathf.Max(0, currentGameTime);

            int totalAttempts =
                correctCount + wrongCount;

            float accuracy =
                totalAttempts > 0
                ? ((float)correctCount / totalAttempts) * 100f : 0f;

            float avgCps =
                survivedTime > 0
                ? (float)totalAttempts / survivedTime: 0f;*/

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
                recordText.text = $"획득 공덕 {earnedMerit}p";
            

                /*recordText.text =
                    $"공덕 {earnedMerit} " +
                    $"생존 {survivedTime:F0}s " +
                    $"정답/오답 {correctCount}/{wrongCount} " +
                    $"정확도 {accuracy:F0}% " +
                    $"평균 CPS {avgCps:F1} " +
                    $"최고 CPS {maxCPS:F0} ";*/
            }
        }

        bool willAutoReturn = GameManager.Instance != null && GameManager.Instance.IsPendingEndingTransition();

        if (restartButton != null)
            restartButton.gameObject.SetActive(!willAutoReturn);
    }

    public void restartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MiniGame_02");
    }

    public void returnToLobby()
    {
        Debug.Log("MG02 returnToLobby 호출");
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager 있음");
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
            float alpha =
                Mathf.PingPong(
                    Time.time * 1.5f,
                    1f
                );

            Color color = startImage.color;
            color.a = alpha;
            startImage.color = color;

            yield return null;
        }
        originalColor.a = 1f;
        startImage.color = originalColor;
    }


    // 시작 Flash
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

            float alpha = Mathf.Lerp(  1f, 0f,  time / duration
                );

            color.a = alpha;
            flashPanel.color = color;

            yield return null;
        }

        color.a = 0f;
        flashPanel.color = color;

        flashPanel.gameObject.SetActive(false);
    }

    // 승객 Idle + Loop
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
            float offset = Mathf.Sin( Time.time * 2f ) * 3f;

            passenger.anchoredPosition = originalPosition +  new Vector2( 0f,  offset );

            yield return null;
        }

        passenger.anchoredPosition =  originalPosition;
    }

    // 정답 Punch Scale
    IEnumerator PunchScaleRoutine(
        Transform target
    )
    {
        if (target == null)
            yield break;

        Vector3 originalScale = target.localScale;

        Vector3 punchScale = originalScale * 1.15f;

        float duration = 0.2f;
        float time = 0f;

        while (time < duration / 2f)
        {
            time += Time.deltaTime;

            float t = time / (duration / 2f);

            target.localScale = Vector3.Lerp( originalScale, punchScale,  t );

            yield return null;
        }

        time = 0f;

        while (time < duration / 2f)
        {
            time += Time.deltaTime;

            float t = time / (duration / 2f);

            target.localScale =Vector3.Lerp( punchScale, originalScale, t );

            yield return null;
        }

        target.localScale = originalScale;
    }

    // 오답 Shake
    IEnumerator ShakeRoutine(
        RectTransform target
    )
    {
        if (target == null)
            yield break;

        Vector2 originalPosition =target.anchoredPosition;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Mathf.Sin( time * 50f) * 10f;

            target.anchoredPosition = originalPosition + new Vector2( x, 0f );

            yield return null;
        }

        target.anchoredPosition = originalPosition;
    }

    // 게이지 Fill
    IEnumerator HealthBarRoutine(
        float targetValue
    )
    {
        if (healthBar == null)
            yield break;

        float startValue = healthBar.value;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t =Mathf.Clamp01( time / duration);

            healthBar.value = Mathf.Lerp( startValue,targetValue,t);
            yield return null;
        }

        healthBar.value = targetValue;
    }

    // 결과창 Scale
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

            // Ease Out Back
            float overshoot = 1.70158f;
            float backT = t - 1f;
            t = backT * backT *
                ((overshoot + 1f) * backT +
                overshoot) + 1f;

            panel.localScale = Vector3.Lerp(Vector3.zero,Vector3.one,t);

            yield return null;
        }

        panel.localScale = Vector3.one;
    }
}