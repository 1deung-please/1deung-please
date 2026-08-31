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

    [Header("Normal Dialogue Position (Checked)")]
    [SerializeField] private float normalPosX = 0f;

    [Header("Character Dialogue Position (Unchecked)")]
    [SerializeField] private float characterPosX = -39.03f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 0.8f; // 페이드 속도 조절

    [Header("UI")]
    [SerializeField] private GameObject skipButton;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeInDuration = 4f;
    [SerializeField] private float bgmFadeOutDuration = 1f;


    [Header("Cafe Animation")]
    [SerializeField] private GameObject cafeBackgroundAnimObject;

    private bool isFading = false;

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
        {
            skipButton.SetActive(tutorialCompleted);

            Image skipImage = skipButton.GetComponent<Image>();
            if (skipImage != null)
            {
                skipImage.alphaHitTestMinimumThreshold = 0.1f;
            }
        }
        else
        {
            Debug.LogError("skipButton이 연결되지 않았습니다.");
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            fadePanel.blocksRaycasts = true;
            StartCoroutine(StartFadeInCoroutine());
        }

        if (bgmSource != null)
        {
            bgmSource.volume = 0f;
            bgmSource.Play();
            StartCoroutine(BGMFadeIn(bgmFadeInDuration));
        }

        StartTutorial();
    }

    public void StartTutorial()
    {
        if (dialogueManager != null)
            dialogueManager.StartDialogue(tutorialDialogue);
    }

    public void SetDialoguePos(bool isNormal)
    {
        if (dialoguePanelRect == null) return;

        Vector2 anchoredPos = dialoguePanelRect.anchoredPosition;

        if (isNormal)
            anchoredPos.x = normalPosX;

        else
            anchoredPos.x = characterPosX;

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

    public void FadeOutAndIn(System.Action onBlackoutAction = null)
    {
        if (isFading) return;

        StartCoroutine(AutoFadeOutInCoroutine(onBlackoutAction));
    }

    public void FadeOut()
    {
        if (isFading) return;

        StartCoroutine(FadeOutCoroutine());
    }

    public void ChangeBGMWithFade(AudioClip newClip)
    {
        StartCoroutine(BGMCrossFade(newClip));
    }

    public void UnlockAchievement(string achievementName)
    {
        Debug.Log("업적 획득 : " + achievementName);
    }

    private IEnumerator AutoFadeOutInCoroutine(System.Action onBlackoutAction)
    {
        isFading = true;

        if (fadePanel != null) fadePanel.blocksRaycasts = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            yield return null;
        }

        if (fadePanel != null) fadePanel.alpha = 1f;

        onBlackoutAction?.Invoke();
        yield return new WaitForSeconds(0.2f);
        time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            yield return null;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        isFading = false;
    }

    private IEnumerator FadeOutCoroutine()
    {
        isFading = true;

        if (fadePanel != null) fadePanel.blocksRaycasts = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            yield return null;
        }

        if (fadePanel != null) fadePanel.alpha = 1f;

        isFading = false;
    }

    private IEnumerator StartFadeInCoroutine()
    {
        isFading = true;
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(1f, 0f, time / duration);

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

    public bool IsFading()
    {
        if (isFading) return true;

        if (fadePanel != null && fadePanel.alpha > 0.1f) return true;

        return false;
    }

    public bool ConsumeClick() => false;

    public void SkipTutorial()
    {
        Debug.Log("스킵 버튼 클릭!");

        if (dialogueManager != null)
            dialogueManager.SkipDialogue();

        MoveLobbyAndStartTimer();
    }

    public void ChangeBGMDirectly(AudioClip newClip)
    {
        if (bgmSource == null || newClip == null) return;

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.volume = 1f;
        bgmSource.Play();
    }

    public void PlayCafeAnimation()
    {
        if (cafeBackgroundAnimObject != null)
        {
            cafeBackgroundAnimObject.SetActive(true);
        }
    }

    public void StopCafeAnimation()
    {
        if (cafeBackgroundAnimObject != null)
        {
            cafeBackgroundAnimObject.SetActive(false);
        }
    }
}