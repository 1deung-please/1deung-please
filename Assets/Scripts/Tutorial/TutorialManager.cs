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

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeInDuration = 4f;
    [SerializeField] private float bgmFadeOutDuration = 1f;

    private bool waitingForClick = false;
    private bool isFading = false;
    private bool consumeClick = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (BackgroundManager.Instance != null) BackgroundManager.Instance.ChangeToTutorial();

        bool tutorialCompleted = false;

        if (GameManager.Instance == null)
            Debug.LogError("GameManager.Instance가 null입니다.");
        else if (GameManager.Instance.gameData == null)
            Debug.LogError("gameData가 null입니다.");
        else
            tutorialCompleted = GameManager.Instance.gameData.tutorialDone;

        if (skipButton != null)
            skipButton.SetActive(tutorialCompleted);
        else
            Debug.LogError("skipButton이 연결되지 않았습니다.");

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        // BGM 시작 시 Fade In
        if (bgmSource != null)
        {
            bgmSource.volume = 0f;
            bgmSource.Play();
            StartCoroutine(BGMFadeIn(bgmFadeInDuration));
        }

        StartTutorial();
    }

    private void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            waitingForClick = false;
            consumeClick = true;
            StartCoroutine(FadeInCoroutine());
        }
    }

    public void StartTutorial()
    {
        if (dialogueManager != null)
            dialogueManager.StartDialogue(tutorialDialogue);
    }

    public void SetDialoguePosY(bool isNarration)
    {
        if (dialoguePanelRect == null) return;
        Vector2 anchoredPos = dialoguePanelRect.anchoredPosition;
        anchoredPos.y = isNarration ? narrationPosY : normalPosY;
        dialoguePanelRect.anchoredPosition = anchoredPos;
    }

    public void MoveLobbyAndStartTimer()
    {
        Debug.Log("MoveLobbyAndStartTimer 실행됨");
        if (GameManager.Instance != null)
            GameManager.Instance.OnTutorialComplete();
        else
            Debug.LogError("GameManager.Instance가 없습니다.");
    }

    public void FadeOut()
    {
        if (isFading) return;
        StartCoroutine(FadeCoroutine());
    }

    // 배경 전환 시 BGM Fade Out → Fade In
    public void ChangeBGMWithFade(AudioClip newClip)
    {
        StartCoroutine(BGMCrossFade(newClip));
    }

    public void UnlockAchievement(string achievementName)
    {
        Debug.Log("업적 획득 : " + achievementName);
    }

    private IEnumerator FadeCoroutine()
    {
        isFading = true;

        if (fadePanel != null)
            fadePanel.blocksRaycasts = true;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, time);
            yield return null;
        }

        if (fadePanel != null)
            fadePanel.alpha = 1f;

        UnlockAchievement("사이비 퇴치!");

        isFading = false;
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
                fadePanel.alpha = Mathf.Lerp(1f, 0f, time);
            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        isFading = false;
    }

    private IEnumerator BGMFadeIn(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            if (bgmSource != null)
                bgmSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
        if (bgmSource != null) bgmSource.volume = 1f;
    }

    private IEnumerator BGMFadeOut(float duration)
    {
        float time = 0f;
        float startVolume = bgmSource != null ? bgmSource.volume : 1f;
        while (time < duration)
        {
            time += Time.deltaTime;
            if (bgmSource != null)
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        if (bgmSource != null)
        {
            bgmSource.volume = 0f;
            bgmSource.Stop();
        }
    }

    private IEnumerator BGMCrossFade(AudioClip newClip)
    {
        yield return StartCoroutine(BGMFadeOut(bgmFadeOutDuration));
        if (bgmSource != null)
        {
            bgmSource.clip = newClip;
            bgmSource.Play();
        }
        yield return StartCoroutine(BGMFadeIn(bgmFadeInDuration));
    }

    public bool IsFading() => isFading;

    public void SkipTutorial()
    {
        Debug.Log("스킵 버튼 클릭!");
        if (dialogueManager != null)
            dialogueManager.SkipDialogue();
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