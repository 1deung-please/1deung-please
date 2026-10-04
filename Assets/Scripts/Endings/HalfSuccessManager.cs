using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HalfSuccessManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialogueUI;

    [Header("Portrait")]
    [SerializeField] private Sprite Ancestor;

    [Header("Try Again")]
    [SerializeField] private Button tryAgainButton;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Background Animation")]
    [SerializeField] private GameObject backgroundAnimObject;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeInDuration = 4f;

    // 이 씬이 담당하는 엔딩 ID - AchievementManager.OnEndingConfirmed에 그대로 전달됨
    private const string EndingId = "절반의성공";

    private bool clickRequested = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(true);
        }

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(false);

        if (bgmSource != null)
        {
            bgmSource.Stop();
            bgmSource.volume = 0f;
        }

        if (backgroundAnimObject != null)
            backgroundAnimObject.SetActive(true);

        StartCoroutine(EndingStart());
    }

    private void Update()
    {
        if (UIInputGate.IsBlocked)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            clickRequested = true;
        }
    }

    private IEnumerator EndingStart()
    {
        yield return Dialogue(
            "조상신",
            "흠... 어디 보자...",
            Ancestor
        );

        PlayBGM();

        yield return Dialogue(
            "조상신",
            "참으로 애~매하구나!",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "열심히 안 한 건 아닌데, 그렇다고 눈물겹게 열심히 한 것도 아니고...",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "딱 주 5일 턱걸이로 출근 도장만 찍은 느낌이구나...",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "그래도 이 팍팍한 세상에 평타라도 친 게 어디냐.",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "네 성의를 봐서 대박 복권까지는 아니어도, 로또 3등 당첨권을 내려주마!",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "감질나느냐? 억울하면 다음엔 눈 딱 감고 풀악셀로 덕 한번 쌓아보거라!",
            Ancestor
        );

        yield return Dialogue(
            "조상신",
            "자, 리스폰 고고!",
            Ancestor
        );

        EndDialogue();
    }

    private IEnumerator Dialogue(string speaker, string text, Sprite portrait)
    {
        clickRequested = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
        {
            if (string.IsNullOrEmpty(speaker))
            {
                nameText.gameObject.SetActive(false);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                nameText.text = speaker;
            }
        }

        SetPortrait(portrait);

        if (dialogueText != null)
            dialogueText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));

        yield return typingCoroutine;

        if (dialogueUI != null)
            dialogueUI.SetActive(true);

        clickRequested = false;

        yield return new WaitUntil(() => clickRequested);

        clickRequested = false;
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        foreach (char c in text)
        {
            if (clickRequested)
            {
                dialogueText.text = text;
                clickRequested = false;
                break;
            }

            dialogueText.text += c;

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void SetPortrait(Sprite portrait)
    {
        if (portraitImage == null)
            return;

        if (portrait == null)
        {
            portraitImage.gameObject.SetActive(false);
            return;
        }

        portraitImage.sprite = portrait;
        portraitImage.gameObject.SetActive(true);
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

    private void EndDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        // 가방에서 "다시보기"로 재생한 경우: 팝업 없이(이미 예전에 확정된 엔딩이므로)
        // 곧바로 로비+가방으로 자동 복귀
        if (GameManager.Instance != null && GameManager.Instance.IsEndingReplay)
        {
            GameManager.Instance.EndEndingReplay();
            return;
        }

        // 배경만 남은 지금 상태에서 엔딩/업적 팝업을 띄우고,
        // 전부 닫힐 때까지 기다린 뒤에야 "다시 도전하기" 버튼을 노출한다.
        StartCoroutine(ConfirmEndingThenShowRetryButton());
    }

    private IEnumerator ConfirmEndingThenShowRetryButton()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnEndingConfirmed(EndingId);

            // 엔딩 팝업 -> (있다면) 업적 팝업 순서로 큐가 처리되고,
            // 사용자가 각 팝업을 터치해서 닫을 때까지 여기서 대기한다.
            yield return new WaitUntil(() => !AchievementManager.Instance.HasPendingPopups);
        }

        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(true);
    }

    // 엔딩 씬 종료 후 메인메뉴(또는 히든 엔딩)로 돌아갈 때
    // - 엔딩/업적 팝업은 이미 위에서 다 처리했으므로, GameManager는 씬 라우팅만 담당
    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenuFromEnding();
        else
            SceneManager.LoadScene("MainMenu");
    }
}