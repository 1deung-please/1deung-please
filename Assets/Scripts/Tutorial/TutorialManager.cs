using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData tutorialDialogue;
    
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
    
        // 튜토리얼 여부 초기화 코드
        //PlayerPrefs.DeleteKey("TutorialCompleted");
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

        // 나중에 업적 UI 연결
    }

    IEnumerator FadeCoroutine()
    {
        isFading = true;

        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(0, 1, time);

            yield return null;
        }

        fadePanel.alpha = 1;

        UnlockAchievement("사이비 퇴치!");

        waitingForClick = true;
    }

    IEnumerator FadeInCoroutine()
    {
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(1, 0, time);

            yield return null;
        }

        fadePanel.alpha = 0;

        isFading = false;
    }
    
    public void SkipTutorial()
    {
        dialogueManager.SkipDialogue();

        MoveLobbyAndStartTimer();
    }
}