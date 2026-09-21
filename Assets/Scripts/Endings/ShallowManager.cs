using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShallowManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialogueUI;

    [Header("Portrait")]
    [SerializeField] private Sprite ancestorAngry;

    [Header("Try Again")]
    [SerializeField] private Button tryAgainButton;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Background Animation")]
    [SerializeField] private GameObject backgroundAnimObject;

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
        if (Input.GetMouseButtonDown(0))
        {
            clickRequested = true;
        }
    }

    private IEnumerator EndingStart()
    {
        yield return Dialogue(
            "조상신",
            "흠... 점수는 괜찮고, 그래 꽤 잘 쌓아왔구나. 그래그래",
            ancestorAngry
        );

        PlayBGM();

        yield return Dialogue(
            "조상신",
            "뭐얏!!!!",
            ancestorAngry
        );

        string mostPlayedGame = GetMostPlayedGame();

        yield return Dialogue(
            "조상신",
            "네 이녀석! 지금까지 진심으로 공덕을 쌓은 것이 아니라 오로지 돈만 바라보며 공덕을 쌓은 것이구나!!!",
            ancestorAngry
        );

        yield return Dialogue(
            "조상신",
            "가장 공덕 쌓기 쉬운 " + mostPlayedGame + "로 공덕 쌓기만 했어!!!!!!",
            ancestorAngry
        );

        yield return Dialogue(
            "조상신",
            "너는 선행을 위한 선행을 한 것이 아니라 오로지 돈만 보고 일을 한 것이로구나!",
            ancestorAngry
        );

        yield return Dialogue(
            "조상신",
            "썩 꺼지거라! 그리고 다시 진심을 다해 공덕을 쌓아오거라!!!",
            ancestorAngry
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

        // 가방에서 "다시보기"로 재생한 경우: 버튼 없이 자동으로 로비+가방으로 복귀
        if (GameManager.Instance != null && GameManager.Instance.IsEndingReplay)
        {
            GameManager.Instance.EndEndingReplay();
            return;
        }

        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(true);
    }

    private string GetMostPlayedGame()
    {
        int max = gameData.playCount[0];
        int index = 0;

        for (int i = 1; i < gameData.playCount.Length; i++)
        {
            if (gameData.playCount[i] > max)
            {
                max = gameData.playCount[i];
                index = i;
            }
        }

        switch (index)
        {
            case 0:
                return "이걸 안 비켜?";

            case 1:
                return "출격! 논리요새";

            case 2:
                return "주워줘, 쓰레기";
        }

        return "";
    }

    // 엔딩 씬 종료 후 메인메뉴로 돌아갈 때 - GameManager를 통해 전환하면
    // 업적/엔딩 팝업이 메인메뉴 전환 완료 후 표시됨
    public void GoToMainMenu()
    {
        bool allNormalEndings =
            EndingStorage.IsUnlocked("자격미달") &&
            EndingStorage.IsUnlocked("절반의성공") &&
            EndingStorage.IsUnlocked("진정한귀인");

        if (allNormalEndings)
        {
            Debug.Log("일반 엔딩 3개 확인 완료 → 히든 엔딩으로 이동");
            SceneManager.LoadScene("Ending_Hidden");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenuFromEnding();
        else
            SceneManager.LoadScene("MainMenu");
    }
}