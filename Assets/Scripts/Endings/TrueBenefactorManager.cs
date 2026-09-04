using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TrueBenefactorManager : MonoBehaviour
{
    public enum EndingAnimation
    {
        None,
        Animation1,
        Animation2
    }

    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialogueUI;

    [Header("Portrait")]
    [SerializeField] private Sprite ancestorGod;
    [SerializeField] private Sprite player;
    [SerializeField] private Sprite dobmitgirl;

    [Header("Ending Story Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite endingStory1;
    [SerializeField] private Sprite endingStory2;
    [SerializeField] private Sprite endingStory3;

    [Header("Try Again")]
    [SerializeField] private Button tryAgainButton;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Background Animation")]
    [SerializeField] private GameObject backgroundAnimObject;
    [SerializeField] private Animator backgroundAnimator;

    [Header("Animation State Names")]
    [SerializeField] private string animation1StateName = "Ending_TrueBenefactor1";
    [SerializeField] private string animation2StateName = "Ending_TrueBenefactor2";

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float bgmFadeInDuration = 4f;

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

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        if (bgmSource != null)
        {
            bgmSource.Stop();
            bgmSource.volume = 0f;
        }

        if (backgroundAnimator == null && backgroundAnimObject != null)
            backgroundAnimator = backgroundAnimObject.GetComponent<Animator>();

        StartBackgroundAnimation(EndingAnimation.Animation1);

        StartCoroutine(EndingStart());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            clickRequested = true;
    }

    private IEnumerator EndingStart()
    {
        PlayBGM();

        yield return Dialogue(
            "조상님",
            "어이쿠야!!!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "매, 맵다 매워! 점수판에서 불이 나는구나!!!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "대박이다, 대박이야! 우리 가문에 드디어 전설의 '미친 재능'이 태어났었구나!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            gameData.meritPoint + "점이라니, 저승 공덕 인플루언서 랭킹 1위 각이로다!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "네 덕에 내가 저승 노인정에서 \"내 후손이 이 정도다!\"라며 어깨뽕을 우주까지 세우고 다닐 수 있게 됐다!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "고맙다, 내 새끼!!!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "주인공",
            "저... 조상님? 그럼 저 정말 로또 1등 되는 건가요?",
            player,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "당연하다마다. 내가 누구냐, 네 조상 아니더냐...",
            ancestorGod,
            EndingAnimation.Animation1
        );

        StartStoryBackground();

        yield return Dialogue(
            "주인공",
            "...",
            player,
            EndingAnimation.None
        );

        ChangeBackground(endingStory2);

        yield return Dialogue(
            "주인공",
            "....",
            player,
            EndingAnimation.None
        );

        ChangeBackground(endingStory3);

        yield return Dialogue(
            "주인공",
            "끙...",
            player,
            EndingAnimation.None
        );

        RestoreBackgroundAnimation(EndingAnimation.Animation1);

        yield return Dialogue(
            "조상님",
            "어린 나이에 집안에 빨간 딱지가 붙어 펑펑 울 때도,",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "돈이 없어 삼각김밥 하나로 하루를 버틸 때도...",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "그리고 매일 회사에서 치이며 '다 때려치고 싶다'고 소리 없는 비명을 지를 때도...",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "난 늘 네 곁에서 가슴을 쥐어짜며 함께 울고 있었다.",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "이 미련한 녀석아, 네가 그동안 얼마나 팍팍하고 외롭게 살아왔는지 내가 다 안다.",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "오죽하면 그 길바닥에서 도믿걸이 말을 걸었는데도 덥석 따라왔겠느냐...",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "주인공",
            "...네? 도믿걸요? 그걸 조상님이 어떻게...",
            player,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "도믿걸",
            "제가원래이런말잘안하는데요, 귀인님 인상이 참 좋으십니다? 후후.",
            dobmitgirl,
            EndingAnimation.Animation2
        );

        yield return Dialogue(
            "주인공",
            "도... 도믿걸?! 당신이 왜 ?!",
            player,
            EndingAnimation.Animation2
        );

        yield return Dialogue(
            "조상님",
            "네가 도통 잠을 안 자 조상 꿈을 안 꿔주니,",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "내가 답답해서 저승 법률 위반해가며 직접 지상으로 다이렉트 중계를 내려간 것이지!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "네 녀석에게 억지로라도 선행을 베풀게 해서, 떳떳하게 대박 복을 내릴 명분을 만들려고 말이다!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "근데 내 예상을 뛰어넘어 이렇게 완벽하게 공덕을 쌓아오다니...",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "역시 내 핏줄다워! 장하다, 내 새끼!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "이제 그 눈물 젖은 삼각김밥과 꼰대 상사는 영원히 안녕이다!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "오냐! 내 감동의 눈물이 앞을 가리는구나!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "저기 황금 카펫 깔린 환생의 문 보이느냐?",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "당당하게 워킹해서 가거라!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        yield return Dialogue(
            "조상님",
            "넌 다음 생... 아니, 이번 생의 당당한 주인공이니까! 웰컴 투 리치 라이프!!!",
            ancestorGod,
            EndingAnimation.Animation1
        );

        EndDialogue();
    }

    private IEnumerator Dialogue(
        string speaker,
        string text,
        Sprite portrait,
        EndingAnimation animation)
    {
        clickRequested = false;

        ChangeAnimation(animation);

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

    private void ChangeAnimation(EndingAnimation animation)
    {
        if (backgroundAnimObject != null)
            backgroundAnimObject.SetActive(animation != EndingAnimation.None);

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(animation == EndingAnimation.None);

        if (animation == EndingAnimation.None)
            return;

        if (backgroundAnimator == null)
        {
            Debug.LogError("Background Animator가 연결되지 않았습니다.");
            return;
        }

        string stateName = GetAnimationStateName(animation);

        if (!backgroundAnimator.HasState(0, Animator.StringToHash(stateName)))
        {
            Debug.LogError("Animator에 '" + stateName + "' State가 없습니다.");
            return;
        }

        backgroundAnimator.Play(stateName, 0, 0f);
    }

    private void StartBackgroundAnimation(EndingAnimation animation)
    {
        ChangeAnimation(animation);
    }

    private void StartStoryBackground()
    {
        if (backgroundAnimObject != null)
            backgroundAnimObject.SetActive(false);

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
            backgroundImage.sprite = endingStory1;
        }
    }

    private void RestoreBackgroundAnimation(EndingAnimation animation)
    {
        ChangeAnimation(animation);
    }

    private string GetAnimationStateName(EndingAnimation animation)
    {
        switch (animation)
        {
            case EndingAnimation.Animation1:
                return animation1StateName;

            case EndingAnimation.Animation2:
                return animation2StateName;

            default:
                return "";
        }
    }

    private void ChangeBackground(Sprite background)
    {
        if (backgroundImage != null && background != null)
            backgroundImage.sprite = background;
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

        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}