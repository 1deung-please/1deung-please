using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MiniGame01Phase { Start, Ready, Countdown, Playing, Result }

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
    public GameObject startPanel;      // 시작 화면 (오버레이 + 타이포 + 터치안내)
    public TMP_Text touchToStartText;  // "터치하여 시작하기" (Blink 대상)
    public GameObject flashPanel;      // 터치 시 Flash 효과용 (흰색, 화면 전체)
    public GameObject readyPanel;      // 조상신 대사 패널
    public GameObject countdownPanel;  // 3,2,1 패널
    public GameObject resultPanel;     // 결과 패널
    public TMP_Text targetText;        // "192개 이상 쓰레기를 줍거라!"
    public TMP_Text countdownText;     // 3,2,1
    public TMP_Text timerText;         // TIME 08:39 (mm:ss)
    public TMP_Text collectCountText;  // "주운 쓰레기 189개"
    public GameObject successImage;   // SUCCESS! 이미지 오브젝트
    public GameObject failImage;       // FAIL 이미지 오브젝트
    public TMP_Text resultRecordText;  // 목표/수집/공덕
    public TMP_Text meritText;         // 공덕
    public Button retryButton; // 다시하기 버튼

    [Header("Play HUD (모래시계/타이머박스/쓰레기봉투 묶음)")]
    public GameObject playHudPanel;

    [Header("쓰레기 팝업 (터치할 때마다 1~5 중 랜덤 등장)")]
    public Sprite[] trashSprites;
    public RectTransform trashPopupParent;
    public Vector2 trashPopupSize = new Vector2(80f, 80f);
    public float trashPopupDuration = 2f;
    public float trashPopupOvershootScale = 1.2f;
    public int trashPopupSortingOrder = 10;

    [Header("오디오")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip buttonSfx;
    public AudioClip trashSfx;

    [Header("Fade")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.25f;

    [Header("결과 패널 등장 딜레이")]
    public float resultPanelDelay = 1f; // 실수 클릭 방지를 위한 결과 패널 등장 지연 시간

    private MiniGame01Phase currentPhase;
    private int targetCount;
    private int currentCount;
    private float remainingTime;
    private Coroutine blinkCoroutine;

    void Start()
    {
        currentPhase = MiniGame01Phase.Start;
        ShowPanel(startPanel);

        if (touchToStartText != null)
            blinkCoroutine = StartCoroutine(BlinkText());

        StartCoroutine(FadeIn());
        if (bgmSource != null) bgmSource.Play();
    }

    void Update()
    {
        switch (currentPhase)
        {
            case MiniGame01Phase.Start:
                if (Input.GetMouseButtonDown(0))
                    StartCoroutine(FlashThenReady());
                break;

            case MiniGame01Phase.Ready:
                if (Input.GetMouseButtonDown(0))
                    StartCoroutine(CountdownRoutine());
                break;

            case MiniGame01Phase.Playing:
                UpdatePlaying();
                break;
        }
    }

    void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;
        fadePanel.alpha = 1f;
        fadePanel.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
    }

    IEnumerator FadeOut()
    {
        if (fadePanel == null) yield break;
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * 1.5f, 1f);
            Color c = touchToStartText.color;
            c.a = alpha;
            touchToStartText.color = c;
            yield return null;
        }
    }

    IEnumerator FlashThenReady()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        if (flashPanel != null)
        {
            Image flashImage = flashPanel.GetComponent<Image>();
            flashPanel.SetActive(true);
            flashImage.color = new Color(1, 1, 1, 1);

            float t = 0;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                flashImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t / 0.3f));
                yield return null;
            }
            flashPanel.SetActive(false);
        }

        currentPhase = MiniGame01Phase.Ready;
        targetCount = Random.Range(minTarget, maxTarget + 1);

        if (targetText != null)
            targetText.text = $"흠... {targetCount}개 이상 쓰레기를 줍거라!";

        ShowPanel(readyPanel);
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMiniGameStart();
            GameManager.Instance.RecordMiniGamePlay(1);
        }

        ShowPanel(null);
        UpdateCollectUI();
    }

    void UpdatePlaying()
    {
        remainingTime -= Time.deltaTime;
        UpdateTimerUI();

        if (Input.GetMouseButtonDown(0))
        {
            currentCount++;
            UpdateCollectUI();
            SpawnTrashPopup(Input.mousePosition);
            PlaySfx(trashSfx);
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            UpdateTimerUI();
            EndGame(false);
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.FloorToInt(remainingTime);
            int centiseconds = Mathf.FloorToInt((remainingTime - seconds) * 100f);
            timerText.text = $"{seconds:00}:{centiseconds:00}";
        }
    }

    void UpdateCollectUI()
    {
        if (collectCountText != null)
            collectCountText.text = $"주운 쓰레기 <size=140%>{currentCount}</size>개";
    }

    void SpawnTrashPopup(Vector2 screenPosition)
    {
        if (trashSprites == null || trashSprites.Length == 0 || trashPopupParent == null)
            return;

        GameObject popup = new GameObject("TrashPopup", typeof(RectTransform), typeof(Image));
        RectTransform rt = popup.GetComponent<RectTransform>();
        rt.SetParent(trashPopupParent, false);

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = trashPopupSize;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            trashPopupParent, screenPosition, GetCanvasCamera(trashPopupParent), out Vector2 localPoint);
        rt.anchoredPosition = localPoint;

        Image img = popup.GetComponent<Image>();
        img.sprite = trashSprites[Random.Range(0, trashSprites.Length)];
        img.raycastTarget = false;

        Canvas canvas = popup.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = trashPopupSortingOrder;

        StartCoroutine(AnimateTrashPopup(rt));
    }

    Camera GetCanvasCamera(RectTransform parent)
    {
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    IEnumerator AnimateTrashPopup(RectTransform rt)
    {
        rt.localScale = Vector3.zero;

        const float overshootPoint = 0.6f;
        float t = 0f;

        while (t < trashPopupDuration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / trashPopupDuration);

            float scale = ratio < overshootPoint
                ? Mathf.Lerp(0f, trashPopupOvershootScale, ratio / overshootPoint)
                : Mathf.Lerp(trashPopupOvershootScale, 1f, (ratio - overshootPoint) / (1f - overshootPoint));

            rt.localScale = Vector3.one * scale;
            yield return null;
        }

        if (rt != null)
            Destroy(rt.gameObject);
    }

    void EndGame(bool naturalEnd)
    {
        currentPhase = MiniGame01Phase.Result;

        bool isSuccess = currentCount >= targetCount;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
            GameManager.Instance.RecordMiniGameResult(1, isSuccess);
            GameManager.Instance.SetPendingAchievementCheck(MiniGameKind.PickTrash, isSuccess);
        }

        int merit = isSuccess
            ? currentCount + successBonus
            : Mathf.RoundToInt(currentCount * failPenaltyRate);

        if (successImage != null) successImage.SetActive(isSuccess);
        if (failImage != null) failImage.SetActive(!isSuccess);

        if (resultRecordText != null)
            resultRecordText.text =
                $"목표:  <size=130%><color=#FFC756>{targetCount}</color></size> 개\n" +
                $"주운 쓰레기 개수:  <size=130%><color=#FFC756>{currentCount}</color></size> 개";

        if (meritText != null)
            meritText.text = $"얻은 공덕 포인트:  <color=#FF69F3>{merit}</color> <size=50>P</size>";

        GameManager.Instance.CompleteMiniGame1(currentCount, targetCount);

        bool willAutoReturn = GameManager.Instance.IsPendingEndingTransition();

        if (retryButton != null)
            retryButton.gameObject.SetActive(!willAutoReturn);

        // 결과 패널은 실수 클릭 방지를 위해 딜레이 후 등장
        StartCoroutine(ShowResultPanelDelayed(resultPanelDelay));
    }

    IEnumerator ShowResultPanelDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPanel(resultPanel);
    }

    void ShowPanel(GameObject target)
    {
        if (startPanel != null) startPanel.SetActive(target == startPanel);
        if (readyPanel != null) readyPanel.SetActive(target == readyPanel);
        if (countdownPanel != null) countdownPanel.SetActive(target == countdownPanel);
        if (resultPanel != null) resultPanel.SetActive(target == resultPanel);
        if (playHudPanel != null) playHudPanel.SetActive(target == null);
    }

    public void OnClickRetry()
    {
        PlaySfx(buttonSfx);
        StartCoroutine(FadeOut());
        StartCoroutine(LoadAfterFade("MiniGame_01"));
    }

    public void OnClickReturnToLobby()
    {
        PlaySfx(buttonSfx);
        StartCoroutine(FadeOut());
        StartCoroutine(LoadAfterFade(null));
    }

    IEnumerator LoadAfterFade(string sceneName)
    {
        yield return new WaitForSeconds(fadeDuration);
        if (sceneName != null)
            SceneLoader.Instance.LoadScene(sceneName);
        else
            GameManager.Instance.ReturnToLobby();
    }
}