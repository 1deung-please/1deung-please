using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData tutorialDialogue;
    
    // 대사창 위치 조정을 위한 RectTransform 및 Y값 설정
    [Header("Dialogue UI Rect Settings")]
    [SerializeField] private RectTransform dialoguePanelRect; // 대사창 패널의 RectTransform
    [SerializeField] private float normalPosY = -300f;        // 일반 대사시 Y 위치
    [SerializeField] private float narrationPosY = -400f;     // 나레이션일 때 Y 위치 (원하는 값으로 조정)

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("UI")]
    [SerializeField] private GameObject skipButton;
    private bool waitingForClick = false;
    private bool isFading = false;

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

        if (skipButton != null)
        {
            skipButton.SetActive(tutorialCompleted);
        }
        else
        {
            Debug.LogError("skipButton이 연결되지 않았습니다.");
        }

        StartTutorial();
    }

    private void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            waitingForClick = false;
            StartCoroutine(FadeInCoroutine());
        }
    }

    public void StartTutorial()
    {
        dialogueManager.StartDialogue(tutorialDialogue);
    }

    // 대사창 Y 위치 변경 함수 (외부나 DialogueManager에서 호출 가능)
    public void SetDialoguePosY(bool isNarration)
    {
        if (dialoguePanelRect == null) return;

        Vector2 anchoredPos = dialoguePanelRect.anchoredPosition;
        anchoredPos.y = isNarration ? narrationPosY : normalPosY;
        dialoguePanelRect.anchoredPosition = anchoredPos;
    }

    public void MoveLobbyAndStartTimer()
    {
        GameManager.Instance.OnTutorialComplete();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine());
    }

    public void UnlockAchievement(string achievementName)
    {
        Debug.Log("업적 획득 : " + achievementName);
    }

    IEnumerator FadeCoroutine()
    {
        isFading = true;
        float time = 0;

        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true; // 페이드 중 클릭 방지
        }

        while (time < 1f)
        {
            time += Time.deltaTime;
            if (fadePanel != null) fadePanel.alpha = Mathf.Lerp(0, 1, time);
            yield return null;
        }

        if (fadePanel != null) fadePanel.alpha = 1;

        UnlockAchievement("사이비 퇴치!");
        waitingForClick = true;
    }

    IEnumerator FadeInCoroutine()
    {
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime;
            if (fadePanel != null) fadePanel.alpha = Mathf.Lerp(1, 0, time);
            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0;
            fadePanel.blocksRaycasts = false; // 이미지와 화면을 다시 클릭할 수 있게 해제
        }

        isFading = false;
    }

    public void SkipTutorial()
    {
        dialogueManager.SkipDialogue();
        MoveLobbyAndStartTimer();
    }
}