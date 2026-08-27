using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShallowManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialogueUI;

    [Header("Try Again")]
    [SerializeField] private Button tryAgainButton;

    [Header("Animation")]
    [SerializeField] private Animator characterAnimator;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool clickRequested = false;

    private Coroutine typingCoroutine;
    private string currentSentence = "";

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(false);

        if (characterAnimator != null)
            characterAnimator.gameObject.SetActive(false);

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
            "조상님",
            "흠... 점수는 괜찮고, 그래 꽤 잘 쌓아왔구나. 그래그래",
            "AncestorGod"
        );

        yield return Dialogue(
            "조상님",
            "뭐얏!!!!",
            "AncestorGod"
        );

        string mostPlayedGame = GetMostPlayedGame();

        yield return Dialogue(
            "조상님",
            "네 이녀석! 지금까지 진심으로 공덕을 쌓은 것이 아니라 오로지 돈만 바라보며 공덕을 쌓은 것이구나!!!",
            "AncestorGod"
        );

        yield return Dialogue(
            "조상님",
            "가장 공덕 쌓기 쉬운 " + mostPlayedGame + "로 공덕 쌓기만 했어!!!!!!",
            "AncestorGod"
        );

        yield return Dialogue(
            "조상님",
            "너는 선행을 위한 선행을 한 것이 아니라 오로지 돈만 보고 일을 한 것이로구나!",
            "AncestorGod"
        );

        yield return Dialogue(
            "조상님",
            "썩 꺼지거라! 그리고 다시 진심을 다해 공덕을 쌓아오거라!!!",
            "AncestorGod"
        );

        EndDialogue();
    }

    private IEnumerator Dialogue(string speaker, string text, string animationName)
    {
        if (nameText != null)
        {
            if (string.IsNullOrEmpty(speaker))
            {
                nameText.gameObject.SetActive(false);

                if (dialogueUI != null)
                    dialogueUI.SetActive(false);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                nameText.text = speaker;

                if (dialogueUI != null)
                    dialogueUI.SetActive(true);
            }
        }

        PlayAnimation(animationName);

        dialogueText.text = "";
        currentSentence = text;
        isTyping = true;
        clickRequested = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));

        yield return typingCoroutine;

        clickRequested = false;

        yield return new WaitUntil(() => clickRequested);

        clickRequested = false;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
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

        isTyping = false;

        if (dialogueUI != null &&
            nameText != null &&
            !string.IsNullOrEmpty(nameText.text))
        {
            dialogueUI.SetActive(true);
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (characterAnimator == null)
            return;

        if (string.IsNullOrEmpty(animationName))
        {
            characterAnimator.gameObject.SetActive(false);
            characterAnimator.enabled = false;
            return;
        }

        characterAnimator.gameObject.SetActive(true);
        characterAnimator.enabled = true;
        characterAnimator.Rebind();
        characterAnimator.Play(animationName, 0, 0f);
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);

        if (characterAnimator != null)
        {
            characterAnimator.enabled = false;
            characterAnimator.gameObject.SetActive(false);
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
                return "<이걸 안 비켜?>";

            case 1:
                return "<출격! 논리요새>";

            case 2:
                return "<주워줘, 쓰레기>";
        }

        return "";
    }
}