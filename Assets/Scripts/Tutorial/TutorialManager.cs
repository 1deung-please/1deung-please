using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData tutorialDialogue;

    [Header("Dialogue UI Rect Settings")]
    [SerializeField] private RectTransform dialoguePanelRect;
    [SerializeField] private float normalPosY = -300f;
    [SerializeField] private float narrationPosY = -400f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("UI")]
    [SerializeField] private GameObject skipButton;

    private bool waitingForClick = false;
    private bool isFading = false;
    private bool consumeClick = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bool tutorialCompleted = false;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다.");
        }
        else if (GameManager.Instance.gameData == null)
        {
            Debug.LogError("gameData가 null입니다.");
        }
        else
        {
            tutorialCompleted = GameManager.Instance.gameData.tutorialDone;
        }

        // 튜토리얼을 한 번 완료했으면 스킵 버튼 활성화
        if (skipButton != null)
        {
            skipButton.SetActive(tutorialCompleted);
        }
        else
        {
            Debug.LogError("skipButton이 연결되지 않았습니다.");
        }

        // 페이드 초기 상태
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        StartTutorial();
    }

    private void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            waitingForClick = false;

            // 이 클릭은 FadeIn만을 위한 클릭
            consumeClick = true;

            StartCoroutine(FadeInCoroutine());
        }
    }

    public void StartTutorial()
    {
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(tutorialDialogue);
        }
    }

    public void SetDialoguePosY(bool isNarration)
    {
        if (dialoguePanelRect == null)
            return;

        Vector2 anchoredPos = dialoguePanelRect.anchoredPosition;
        anchoredPos.y = isNarration ? narrationPosY : normalPosY;
        dialoguePanelRect.anchoredPosition = anchoredPos;
    }

    public void MoveLobbyAndStartTimer()
    {
        Debug.Log("MoveLobbyAndStartTimer 실행됨");

        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager 발견 → 타이머 시작");
            GameManager.Instance.OnTutorialComplete();
        }
        else
        {
            Debug.LogError("GameManager.Instance가 없습니다.");
        }
    }

    public void FadeOut()
    {
        // 이미 페이드 중이면 중복 실행 방지
        if (isFading)
            return;

        StartCoroutine(FadeCoroutine());
    }

    public void UnlockAchievement(string achievementName)
    {
        Debug.Log("업적 획득 : " + achievementName);
    }

    private IEnumerator FadeCoroutine()
    {
        isFading = true;

        float time = 0f;

        // 페이드 중에는 화면 클릭을 막음
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
        }

        while (time < 1f)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
            {
                fadePanel.alpha = Mathf.Lerp(0f, 1f, time);
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
        }

        UnlockAchievement("사이비 퇴치!");

        // 페이드 완료
        isFading = false;

        // 검은 화면에서 클릭을 기다림
        waitingForClick = true;
    }

    private IEnumerator FadeInCoroutine()
    {
        isFading = true;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
            {
                fadePanel.alpha = Mathf.Lerp(1f, 0f, time);
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        isFading = false;
    }

    public bool IsFading()
    {
        return isFading;
    }

    public void SkipTutorial()
    {
        Debug.Log("스킵 버튼 클릭!");

        if (dialogueManager != null)
        {
            dialogueManager.SkipDialogue();
        }

        MoveLobbyAndStartTimer();
    }

    public bool ConsumeClick()
    {
        if (consumeClick)
        {
            consumeClick = false;
            return true;
        }

        return false;
    }
}