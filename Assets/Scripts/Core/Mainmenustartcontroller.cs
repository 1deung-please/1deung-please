using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuStartController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text touchToStartText;   // "복권 긁으러 가기" 
    public RectTransform backgroundPanel; // 슬라이드인 대상 
    public CanvasGroup titleImage;       // "1등되게 해주세요!" 타이포 이미지 

    [Header("BGM")]
    public AudioSource bgmSource;        // BGM AudioSource

    [Header("이동할 씬 이름")]
    public string tutorialSceneName = "Tutorial";

    [Header("깜빡임 속도 (낮을수록 천천히)")]
    public float blinkSpeed = 0.8f;

    [Header("연출 설정")]
    public float slideDuration = 0.8f;    // 배경 슬라이드인 시간 (초)
    public float titleDelay = 1.0f;       // 배경 슬라이드 후 타이포 등장까지 대기 (초)
    public int flashCount = 3;            // 타이포 번쩍임 횟수
    public float flashInterval = 0.12f;   // 번쩍임 간격 (초)

    private Coroutine blinkCoroutine;
    private bool isTransitioning = false;
    private bool introComplete = false;

    void Start()
    {
        // 메인메뉴에서는 전역 타이머 정지
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
        }

        // 초기 상태: 배경 패널 화면 왼쪽 바깥에, 타이포/텍스트 숨김
        if (backgroundPanel != null)
        {
            Vector2 pos = backgroundPanel.anchoredPosition;
            pos.x = -Screen.width; // 화면 왼쪽 밖으로
            backgroundPanel.anchoredPosition = pos;
        }

        if (titleImage != null)
            titleImage.alpha = 0f;

        if (touchToStartText != null)
        {
            Color c = touchToStartText.color;
            c.a = 0f;
            touchToStartText.color = c;
        }

        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (!introComplete || isTransitioning) return;

        // 업적/엔딩 팝업이 떠 있는 동안에는 화면 클릭을 튜토리얼 진입으로 처리하지 않음
        if (AchievementManager.Instance != null && AchievementManager.Instance.IsPopupActive)
            return;

        // 게임 종료 확인 팝업이 떠 있는 동안에도 마찬가지
        if (ExitPopupManager.Instance != null && ExitPopupManager.Instance.IsPopupActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isTransitioning = true;

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStartGame();
            }
            else
            {
                Debug.LogError("GameManager.Instance가 없습니다.");
            }
        }
    }

    // 아래 연출(슬라이드/대기/번쩍임/깜빡임)은 전부 unscaled 시간을 쓴다.
    // 종료 팝업이 떠서 Time.timeScale이 0이 되어도 메인메뉴 연출은 멈추지 않고 끝까지 함께 진행되게 하기 위함.
    // (메인메뉴에는 멈춰야 할 게임 진행이 없고, 일부만 멈추면 슬라이드는 서 있는데 깜빡임만 도는 식으로 어긋남)
    IEnumerator IntroSequence()
    {
        // BGM 시작과 동시에 배경 슬라이드인
        if (bgmSource != null) bgmSource.Play();

        yield return StartCoroutine(SlideIn());

        // titleDelay초 대기 후 타이포 번쩍 등장
        yield return new WaitForSecondsRealtime(titleDelay);

        yield return StartCoroutine(FlashTitle());

        // "복권 긁으러 가기" 텍스트도 동시에 페이드인
        if (touchToStartText != null)
        {
            Color c = touchToStartText.color;
            c.a = 1f;
            touchToStartText.color = c;
        }

        introComplete = true;
        blinkCoroutine = StartCoroutine(BlinkText());
    }

    IEnumerator SlideIn()
    {
        if (backgroundPanel == null) yield break;

        float elapsed = 0f;
        float startX = -Screen.width * 2f;
        float endX = 0f;

        while (elapsed < slideDuration)
        {
            // 씬이 막 로드된 첫 프레임은 델타타임이 매우 클 수 있다(로딩 시간이 통째로 들어옴).
            // unscaledDeltaTime을 그대로 더하면 첫 프레임에 슬라이드가 끝나버려 부드럽게 들어오지 않으므로
            // 프레임당 증가량에 상한을 둔다. (평소 프레임에서는 상한에 안 걸려 deltaTime과 똑같이 동작)
            elapsed += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration); // 부드럽게
            Vector2 pos = backgroundPanel.anchoredPosition;
            pos.x = Mathf.Lerp(startX, endX, t);
            backgroundPanel.anchoredPosition = pos;
            yield return null;
        }

        Vector2 finalPos = backgroundPanel.anchoredPosition;
        finalPos.x = endX;
        backgroundPanel.anchoredPosition = finalPos;
    }

    IEnumerator FlashTitle()
    {
        if (titleImage == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            titleImage.alpha = 1f;
            yield return new WaitForSecondsRealtime(flashInterval);
            titleImage.alpha = 0f;
            yield return new WaitForSecondsRealtime(flashInterval);
        }
        titleImage.alpha = 1f; // 마지막엔 완전히 켜진 상태로 고정
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f);
            Color c = touchToStartText.color;
            c.a = alpha;
            touchToStartText.color = c;
            yield return null;
        }
    }
}