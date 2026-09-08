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

    IEnumerator IntroSequence()
    {
        // BGM 시작과 동시에 배경 슬라이드인
        if (bgmSource != null) bgmSource.Play();

        yield return StartCoroutine(SlideIn());

        // titleDelay초 대기 후 타이포 번쩍 등장
        yield return new WaitForSeconds(titleDelay);

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
            elapsed += Time.deltaTime;
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
            yield return new WaitForSeconds(flashInterval);
            titleImage.alpha = 0f;
            yield return new WaitForSeconds(flashInterval);
        }
        titleImage.alpha = 1f; // 마지막엔 완전히 켜진 상태로 고정
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color c = touchToStartText.color;
            c.a = alpha;
            touchToStartText.color = c;
            yield return null;
        }
    }
}