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
        bool tutorialCompleted =
            PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        skipButton.SetActive(tutorialCompleted);

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

    public void MoveLobby()
    {
        Debug.Log("▶ 로비 이동");

        // 나중에 플레이어 이동 코드 작성
    }

    public void StartGameTimer()
    {
        Debug.Log("▶ 5분 타이머 시작");

        // 나중에 타이머 실행
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

        MoveLobby();
    }
}