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

    [Header("BGM (배경 전환 시 Fade)")]
    [SerializeField] private AudioClip streetBGM;
    [SerializeField] private AudioClip cafeBGM;

    private DialogueLine currentLine;
    private int currentIndex = 0;
    private bool dialogueStarted = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence = "";

    public System.Action OnDialogueFinished;

    private void Awake()
    {
        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueData data)
    {
        dialogueData = data;
        currentIndex = 0;
        dialogueStarted = false;

        nameText.gameObject.SetActive(false);
        dialogueText.text = "";

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        gameObject.SetActive(true);

        if (data != null && data.lines.Count > 0 && TutorialManager.Instance != null)
            TutorialManager.Instance.SetDialoguePosY(data.lines[0].speaker == "Narration");
    }

    private void Update()
    {
        if (TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.IsFading()) return;
            if (TutorialManager.Instance.ConsumeClick()) return;
        }

        if (choicePanel != null && choicePanel.activeSelf) return;

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
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
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
        if (dialogueData == null || dialogueData.lines == null || currentIndex >= dialogueData.lines.Count)
        {
            EndDialogue();
            return;
        }

        currentLine = dialogueData.lines[currentIndex];
        DialogueLine line = currentLine;

        nameText.text = line.speaker;

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.SetDialoguePosY(line.speaker == "Narration");

        if (line.speaker == "Narration")
            nameText.gameObject.SetActive(false);
        else
        {
            nameText.gameObject.SetActive(true);
            nameText.text = line.speaker;
        }

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
                portraitImage.gameObject.SetActive(false);
        }

        currentSentence = line.text;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(currentSentence));

        ExecuteEvent(line.dialogueEvent);

        if (choicePanel != null)
            choicePanel.SetActive(line.isChoice);
    }

    private IEnumerator TypeText(string text)
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
        Debug.Log("대화 종료");
        OnDialogueFinished?.Invoke();

        if (TutorialManager.Instance != null)
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();
            Debug.Log("튜토리얼 완료");
            TutorialManager.Instance.MoveLobbyAndStartTimer();
        }
    }

    private void ExecuteEvent(DialogueEvent dialogueEvent)
    {
        switch (dialogueEvent)
        {
            case DialogueEvent.None:
                break;

            case DialogueEvent.MoveLobby:
                TutorialManager.Instance.MoveLobbyAndStartTimer();
                break;

            case DialogueEvent.StartTimer:
                break;

            case DialogueEvent.FadeOut:
                TutorialManager.Instance.FadeOut();
                break;

            case DialogueEvent.Achievement:
                TutorialManager.Instance.UnlockAchievement("사이비 퇴치!");
                break;

            case DialogueEvent.ChangeStreet:
                BackgroundManager.Instance.ChangeToStreet();
                if (TutorialManager.Instance != null && streetBGM != null)
                    TutorialManager.Instance.ChangeBGMWithFade(streetBGM);
                break;

            case DialogueEvent.ChangeCafe:
                BackgroundManager.Instance.ChangeToCafe();
                if (TutorialManager.Instance != null && cafeBGM != null)
                    TutorialManager.Instance.ChangeBGMWithFade(cafeBGM);
                break;

            case DialogueEvent.ChangeToTimer:
                BackgroundManager.Instance.ChangeToTimer();
                break;
        }
    }

    public void OnChoiceResult(bool yes, int noCount = 0)
    {
        if (yes)
            NextDialogue();
        else
            AchievementManager.Instance.OnTutorialNoButtonClicked();
    }

    public void RepeatCurrentDialogue(string speaker, Sprite portrait, string newText)
    {
        currentSentence = newText;
        nameText.gameObject.SetActive(true);

        if (portraitImage != null && portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    public void SkipDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        gameObject.SetActive(false);
    }
}