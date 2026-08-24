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
    public TMP_Text resultReasonText;  // 성공/실패
    public TMP_Text resultRecordText;  // 목표/수집/공덕
    public TMP_Text meritText;         // 공덕
    public Button retryButton; // 다시하기 버튼

    [Header("Play HUD (모래시계/타이머박스/쓰레기봉투 묶음)")]
    // 모래시계, 모래시계 배경, 타이머 박스 외곽/내곽, 쓰레기 봉투, 쓰레기 봉투 배경 박스는
    // 전부 이 오브젝트의 자식으로 배치하면 됨. 개별 로직이 필요 없는 정적 이미지라
    // 스크립트에서 따로 참조하지 않고, 이 부모 오브젝트를 Playing 단계에서만 켜고 끔.
    public GameObject playHudPanel;

    [Header("쓰레기 팝업 (터치할 때마다 1~5 중 랜덤 등장)")]
    public Sprite[] trashSprites;           // 쓰레기 1~5 스프라이트 5개 등록
    public RectTransform trashPopupParent;  // 팝업이 생길 부모 RectTransform (Play HUD가 속한 Canvas 하위 권장)
    public Vector2 trashPopupSize = new Vector2(80f, 80f);
    public float trashPopupDuration = 0.2f; // 튀어나오는 모션 지속시간
    public float trashPopupOvershootScale = 1.2f; // 튀어나올 때 살짝 커졌다가 원래 크기로 정착
    public int trashPopupSortingOrder = 10; // 다른 UI(박스 등)보다 항상 위에 그려지도록

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

        // 화면 Flash 효과
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

        // 조상신 대사 화면(Ready)으로 이동 + 목표 개수 산정
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

        // 새로 만든 RectTransform은 기본이 부모를 꽉 채우는 Stretch 앵커라
        // sizeDelta가 절대 크기로 취급되도록 앵커/피벗을 점(Point)으로 고정
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

        // 박스 등 다른 UI에 가려지지 않도록 개별 Canvas로 정렬 순서를 강제로 높임
        Canvas canvas = popup.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = trashPopupSortingOrder;

        StartCoroutine(AnimateTrashPopup(rt));
    }

    // Canvas Render Mode가 Overlay가 아니면 좌표 변환에 카메라가 필요함
    Camera GetCanvasCamera(RectTransform parent)
    {
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        // Canvas의 Render Camera가 비어있는 경우를 대비한 대체값
        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    // 터치 지점에서 구긴 종이 쓰레기 아이콘이 튀어나오는 듯한 모션 (0 -> 살짝 오버슈트 -> 1 스케일)
    // 모션이 끝나면 바로 사라짐
    IEnumerator AnimateTrashPopup(RectTransform rt)
    {
        rt.localScale = Vector3.zero;

        const float overshootPoint = 0.6f; // 전체 duration 중 튀어나오는 구간 비율
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
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnMiniGameResult(MiniGameKind.PickTrash, isSuccess);
        }

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

        GameManager.Instance.CompleteMiniGame1(currentCount, targetCount);

        bool willAutoReturn = GameManager.Instance.IsPendingEndingTransition();

        // 자동 복귀 예정이면 다시하기 버튼 비활성화
        if (retryButton != null)
            retryButton.gameObject.SetActive(!willAutoReturn);
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
        SceneLoader.Instance.LoadScene("MiniGame_01");
    }

    public void OnClickReturnToLobby()
    {
        GameManager.Instance.ReturnToLobby();
    }

}