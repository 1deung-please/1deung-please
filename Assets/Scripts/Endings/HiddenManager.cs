using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HiddenManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialogueUI;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite ancestorGod;
    [SerializeField] private Sprite player;
    [SerializeField] private Sprite dobmitgirl;
    [SerializeField] private Sprite narration;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2.5f;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeInDuration = 4f;

    [Header("Credits Image")]
    [SerializeField] private Image creditsImage;
    [SerializeField] private float creditsSpeed = 100f;
    [SerializeField] private float mouseWheelSpeed = 3000f;   // PC 휠 스크롤 감도 (선택)
    [SerializeField] private float resumeDelay = 0f;

    private const string EndingId = "히든";

    private bool isTyping = false;
    private bool clickRequested = false;
    private bool endingFinished = false;

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (nameText != null)
            nameText.text = "";

        if (creditsImage != null)
            creditsImage.gameObject.SetActive(false);

        StartCoroutine(EndingStart());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickRequested = true;

            if (endingFinished)
            {
                endingFinished = false;

                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.IsEndingReplay)
                        GameManager.Instance.EndEndingReplay();
                    else
                        GameManager.Instance.ReturnToMainMenuFromEnding();
                }
            }
        }
    }

    private IEnumerator EndingStart()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }

        PlayBGM();

        yield return StartCoroutine(FirstDialogue());

        yield return Dialogue(
            "주인공",
            "플레이 해주셔서 진심으로 감사합니다!",
            player
        );

        yield return Dialogue(
            "주인공",
            "플레이어님은 게임에서 뿐만 아니라, 진정한 귀인이세요!",
            player
        );

        yield return Dialogue(
            "주인공",
            "여기, 열심히 플레이 해주신 당신께 주는 선물입니다.",
            player
        );

        yield return Dialogue(
            "조상신",
            "사실 이 게임에서 가장 얻기 어려운 건 1등 당첨이 아니라...",
            ancestorGod
        );

        yield return Dialogue(
            "조상신",
            "엔딩 4개를 모두 보는 것이었답니다!",
            ancestorGod
        );

        yield return Dialogue(
            "주인공",
            "그러니 오늘만큼은 당당하게 말하세요.",
            player
        );

        yield return Dialogue(
            "주인공",
            "나는 운 좋은 사람이다!",
            player
        );

        yield return Dialogue(
            "조상신",
            "언젠가 현실에서도 좋은 일이 찾아오길 바랍니다.",
            ancestorGod
        );

        yield return Dialogue(
            "도믿걸",
            "그리고...",
            dobmitgirl
        );

        yield return Dialogue(
            "도믿걸",
            "다음에 복권을 긁게 된다면,",
            dobmitgirl
        );

        yield return Dialogue(
            "주인공",
            "조상님 대신 저희가 응원하고 있을게요!",
            player
        );

        yield return Dialogue(
            "전원",
            "1등 되게 해주세요!!",
            narration
        );

        yield return StartCoroutine(StartCredits());
    }

    private IEnumerator FirstDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.text = "???";

        if (portraitImage != null)
        {
            if (narration != null)
            {
                portraitImage.sprite = narration;
                portraitImage.gameObject.SetActive(true);
            }
        }

        if (dialogueText != null)
            dialogueText.text = "";

        clickRequested = false;
        isTyping = true;

        string text = "엔딩 4개를 다 보셨군요!";

        foreach (char c in text)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(true);

        clickRequested = false;

        yield return new WaitUntil(() => clickRequested);

        clickRequested = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (nameText != null)
            nameText.text = "";

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        yield return new WaitUntil(() => clickRequested);

        clickRequested = false;

        yield return StartCoroutine(FadeInFromBlack());
    }

    private IEnumerator Dialogue(string speaker, string text, Sprite portrait)
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.text = speaker;

        if (portraitImage != null)
        {
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        if (dialogueText != null)
            dialogueText.text = "";

        isTyping = true;
        clickRequested = false;

        foreach (char c in text)
        {
            if (clickRequested)
            {
                if (dialogueText != null)
                    dialogueText.text = text;

                clickRequested = false;
                break;
            }

            if (dialogueText != null)
                dialogueText.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(true);

        clickRequested = false;

        yield return new WaitUntil(() => clickRequested);

        clickRequested = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);
    }

    private IEnumerator StartCredits()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (nameText != null)
            nameText.text = "";

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        yield return StartCoroutine(FadeToBlack());

        if (creditsImage != null)
        {
            creditsImage.gameObject.SetActive(true);
            ConfigureCreditsImage();
        }

        yield return StartCoroutine(ScrollCredits());

        bool isReplay =
            GameManager.Instance != null &&
            GameManager.Instance.IsEndingReplay;

        if (!isReplay && AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnEndingConfirmed(EndingId);

            yield return new WaitUntil(
                () => !AchievementManager.Instance.HasPendingPopups
            );
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsEndingReplay)
                GameManager.Instance.EndEndingReplay();
            else
                GameManager.Instance.ReturnToMainMenuFromEnding();
        }
    }

    private void ConfigureCreditsImage()
    {
        if (creditsImage == null)
            return;

        RectTransform rect = creditsImage.rectTransform;
        RectTransform parentRect = rect.parent as RectTransform;

        if (parentRect == null)
            return;

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);

        rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
        rect.offsetMax = new Vector2(0f, rect.offsetMax.y);

        if (creditsImage.sprite != null)
        {
            float imageWidth = parentRect.rect.width;

            float aspectRatio =
                creditsImage.sprite.rect.height /
                creditsImage.sprite.rect.width;

            float imageHeight = imageWidth * aspectRatio;

            rect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                imageHeight
            );
        }

        creditsImage.preserveAspect = true;
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
            yield break;

        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(
                0f,
                1f,
                time / fadeDuration
            );

            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator FadeInFromBlack()
    {
        if (fadeImage == null)
            yield break;

        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(
                1f,
                0f,
                time / fadeDuration
            );

            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator ScrollCredits()
    {
        if (creditsImage == null)
            yield break;

        RectTransform rect = creditsImage.rectTransform;
        RectTransform parentRect = rect.parent as RectTransform;

        if (parentRect == null)
            yield break;

        Canvas canvas = parentRect.GetComponentInParent<Canvas>();
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        float viewportHeight = parentRect.rect.height;
        float imageHeight = rect.rect.height;

        float minY = -viewportHeight;   // 크레딧이 화면 아래에 막 걸쳐 있는 위치
        float maxY = imageHeight;       // 크레딧이 화면 위로 완전히 사라진 위치

        Vector2 position = rect.anchoredPosition;
        position.y = minY;
        rect.anchoredPosition = position;

        bool isDragging = false;
        Vector2 lastLocalPoint = Vector2.zero;
        float resumeTimer = 0f;

        while (true)
        {
            // 드래그 시작 (터치도 Input.GetMouseButton으로 처리됨)
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect, Input.mousePosition, uiCamera, out lastLocalPoint))
                {
                    isDragging = true;
                }
            }

            // 드래그 중: 손가락을 따라 이동
            if (isDragging && Input.GetMouseButton(0))
            {
                Vector2 currentLocalPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect, Input.mousePosition, uiCamera, out currentLocalPoint))
                {
                    float deltaY = currentLocalPoint.y - lastLocalPoint.y;
                    lastLocalPoint = currentLocalPoint;

                    position = rect.anchoredPosition;
                    position.y = Mathf.Clamp(position.y + deltaY, minY, maxY);
                    rect.anchoredPosition = position;
                }
            }
            // 드래그 종료
            else if (isDragging)
            {
                isDragging = false;
                resumeTimer = resumeDelay;
            }

            // 마우스 휠 (PC) - 휠 위로 = 내용 아래로, 휠 아래로 = 내용 위로
            float wheel = Input.mouseScrollDelta.y;
            if (!isDragging && Mathf.Abs(wheel) > 0.01f)
            {
                position = rect.anchoredPosition;
                position.y = Mathf.Clamp(
                    position.y - wheel * mouseWheelSpeed * Time.deltaTime,
                    minY, maxY);
                rect.anchoredPosition = position;
                resumeTimer = resumeDelay;
            }

            // 드래그 중이 아닐 때만 자동 스크롤
            if (!isDragging)
            {
                if (resumeTimer > 0f)
                {
                    resumeTimer -= Time.deltaTime;
                }
                else
                {
                    position = rect.anchoredPosition;
                    position.y += creditsSpeed * Time.deltaTime;
                    rect.anchoredPosition = position;

                    // 끝까지 올라가면 종료
                    if (position.y >= maxY)
                        break;
                }
            }

            yield return null;
        }
    }

    private void PlayBGM()
    {
        if (bgmSource == null)
            return;

        bgmSource.volume = 0f;
        bgmSource.Play();

        StartCoroutine(BGMFadeIn());
    }

    private IEnumerator BGMFadeIn()
    {
        float time = 0f;

        while (time < bgmFadeInDuration)
        {
            time += Time.deltaTime;

            if (bgmSource != null)
            {
                bgmSource.volume = Mathf.Lerp(
                    0f,
                    1f,
                    time / bgmFadeInDuration
                );
            }

            yield return null;
        }

        if (bgmSource != null)
            bgmSource.volume = 1f;
    }
}