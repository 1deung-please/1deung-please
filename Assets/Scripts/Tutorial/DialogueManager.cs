using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogueData;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private GameObject choicePanel;

    [SerializeField] private ChoiceManager choiceManager;

    private DialogueLine currentLine;

    private int currentIndex = 0;

    private bool dialogueStarted = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence = "";

    public void StartDialogue(DialogueData data)
    {
        dialogueData = data;

        currentIndex = 0;
        dialogueStarted = false;

        nameText.gameObject.SetActive(false);
        dialogueText.text = "";

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        choicePanel.SetActive(false);
    }

    private void Update()
    {
        // 선택지가 떠 있으면 대사 넘기기 금지
        if (choicePanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!dialogueStarted)
            {
                dialogueStarted = true;
                ShowDialogue();
                return;
            }

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentSentence;
                isTyping = false;
            }
            else
            {
                NextDialogue();
            }
        }
    }

    private void ShowDialogue()
    {
        if (currentIndex >= dialogueData.lines.Count)
        {
            EndDialogue();
            return;
        }

        currentLine = dialogueData.lines[currentIndex];
        DialogueLine line = currentLine;
        ExecuteEvent(line.dialogueEvent);

        // 이름 표시
        nameText.text = line.speaker;

        // 내레이션 처리
        if (line.speaker == "Narration")
        {
            nameText.gameObject.SetActive(false);
            portraitImage.gameObject.SetActive(false);
        }
        else
        {
            nameText.gameObject.SetActive(true);
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = line.portrait;
        }

        currentSentence = line.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentSentence));

        ExecuteEvent(line.dialogueEvent);

        if (line.isChoice)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            choicePanel.SetActive(false);
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void NextDialogue()
    {
        currentIndex++;
        ShowDialogue();
    }

    public void EndDialogue()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        Debug.Log("튜토리얼 완료");
    }

    void ExecuteEvent(DialogueEvent dialogueEvent)
    {
        switch (dialogueEvent)
        {
            case DialogueEvent.None:
                break;

            case DialogueEvent.MoveLobby:
                TutorialManager.Instance.MoveLobbyAndStartTimer();
                break;

            case DialogueEvent.StartTimer:
                // 이미 MoveLobby 쪽에서 다 처리되니, 이 case는 비워두거나 지워도 됨
                break;

            case DialogueEvent.FadeOut:
                TutorialManager.Instance.FadeOut();
                break;

            case DialogueEvent.Achievement:
                TutorialManager.Instance.UnlockAchievement("사이비 퇴치!");
                break;

            case DialogueEvent.ChangeStreet:
                BackgroundManager.Instance.ChangeToStreet();
                break;

            case DialogueEvent.ChangeCafe:
                BackgroundManager.Instance.ChangeToCafe();
                break;
        }
    }

    public void OnChoiceResult(bool yes, int noCount = 0)
    {
        if (yes)
        {
            NextDialogue();
        }
    }

    public void RepeatCurrentDialogue(string newText)
    {
        currentSentence = newText;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentSentence));
    }

    public void SkipDialogue()
    {
        // 타이핑 중이면 중지
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;

        // 선택지 숨기기
        if (choicePanel != null)
            choicePanel.SetActive(false);

        // 대화 UI 숨기기
        gameObject.SetActive(false);
    }
}