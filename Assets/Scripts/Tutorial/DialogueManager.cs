using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject dialogueUI;

    [Header("Animation")]
    [SerializeField] private Animator characterAnimator;

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

    private string[] branchTexts;
    private string[] branchSpeakers;
    private Sprite[] branchPortraits;
    private int branchIndex = 0;
    private bool isBranchDialogue = false;
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

                if (dialogueUI != null) dialogueUI.SetActive(true);
            }

            else
            {
                if (dialogueUI != null)
                    dialogueUI.SetActive(false);

                if (isBranchDialogue)
                {
                    branchIndex++;
                    ShowCurrentBranchDialogue();
                }
                else
                {
                    NextDialogue();
                }
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

        bool isBackgroundChange = 
            line.dialogueEvent == DialogueEvent.ChangeStreet || 
            line.dialogueEvent == DialogueEvent.ChangeCafe;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.SetDialoguePos(currentLine.isNormalDialogue);
        }

        if (nameText != null)
        {
            if (string.IsNullOrEmpty(line.speaker))
            {
                nameText.gameObject.SetActive(false);

                if (dialogueUI != null)
                    dialogueUI.SetActive(false);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                nameText.text = line.speaker;
            }
        }

        if (portraitImage != null)
        {
            if (line.portrait == null)
            {
                portraitImage.gameObject.SetActive(false);
            }
            else
            {
                portraitImage.sprite = line.portrait;

                if (!isBackgroundChange)
                {
                    portraitImage.gameObject.SetActive(true);
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }
        }

        if (dialogueUI != null) dialogueUI.SetActive(false);

        currentSentence = line.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (isBackgroundChange)
        {
            ToggleDialogueUI(false);

            if (dialogueText != null)
                dialogueText.text = "";

            ExecuteEvent(line.dialogueEvent);
            StartCoroutine(ShowDialogueAfterFade(line));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeText(currentSentence));
            ExecuteEvent(line.dialogueEvent);
        }
        ExecuteAnimEvent(line.animEvent);

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

        if (dialogueUI != null && currentLine != null &&
            !string.IsNullOrEmpty(currentLine.speaker))
        {
            dialogueUI.SetActive(true);
        }   
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
                if (TutorialManager.Instance != null)
                {
                    ToggleDialogueUI(false);

                    TutorialManager.Instance.FadeOutAndIn(() =>
                    {
                        BackgroundManager.Instance.ChangeToStreet();
                        TutorialManager.Instance.StopCafeAnimation();

                        if (streetBGM != null)
                            TutorialManager.Instance.ChangeBGMDirectly(streetBGM);
                    });
                }
                break;

            case DialogueEvent.ChangeCafe:
                if (TutorialManager.Instance != null)
                {
                    ToggleDialogueUI(false);

                    TutorialManager.Instance.FadeOutAndIn(() =>
                    {
                        BackgroundManager.Instance.ChangeToCafe();
                        TutorialManager.Instance.PlayCafeAnimation();

                        if (cafeBGM != null)
                            TutorialManager.Instance.ChangeBGMDirectly(cafeBGM);
                    });
                }
                break;

            case DialogueEvent.ChangeToTimer:
                BackgroundManager.Instance.ChangeToTimer();
                break;
        }
    }

    private void ToggleDialogueUI(bool show)
    {
        if (nameText != null) nameText.gameObject.SetActive(show);
        if (dialogueText != null) dialogueText.gameObject.SetActive(show);

        if (portraitImage != null)
        {
            if (!show)
            {
                portraitImage.gameObject.SetActive(false);
            }
            else
            {
                if (currentLine != null && (currentLine.portrait != null || currentLine.animEvent != DialogueAnimEvent.None))
                {
                    portraitImage.gameObject.SetActive(true);
                }
            }
        }

        if (dialogueUI != null) dialogueUI.SetActive(show);
    }

    private void ExecuteAnimEvent(DialogueAnimEvent animEvent)
    {
        if (characterAnimator == null) return;

        switch (animEvent)
        {
            case DialogueAnimEvent.None:
                characterAnimator.enabled = false;
                break;

            case DialogueAnimEvent.PlayerAnim:
                characterAnimator.gameObject.SetActive(true);
                characterAnimator.enabled = true;
                characterAnimator.Rebind();
                characterAnimator.Play("Player", 0, 0f);
                break;

            case DialogueAnimEvent.DobmitgirlAnim:
                characterAnimator.gameObject.SetActive(true);
                characterAnimator.enabled = true;
                characterAnimator.Rebind();
                characterAnimator.Play("Dobmitgirl", 0, 0f);
                break;

            case DialogueAnimEvent.Dobmitgir_angry_dialAnim:
                characterAnimator.gameObject.SetActive(true);
                characterAnimator.enabled = true;
                characterAnimator.Rebind();
                characterAnimator.Play("Dobmitgirl_angry_dia", 0, 0f);
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

        if (nameText != null)
        {
            nameText.text = speaker;
            nameText.gameObject.SetActive(true);
        }

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

    public void StartBranchDialogue(
        string[] texts,
        string[] speakers,
        Sprite[] portraits)
    {
        if (texts == null || texts.Length == 0)
            return;

        branchTexts = texts;
        branchSpeakers = speakers;
        branchPortraits = portraits;

        branchIndex = 0;
        isBranchDialogue = true;

        ShowCurrentBranchDialogue();
    }

    private void ShowCurrentBranchDialogue()
    {
        if (branchIndex >= branchTexts.Length)
        {
            isBranchDialogue = false;
            branchIndex = 0;
            return;
        }

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.SetDialoguePos(true);
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = branchSpeakers[branchIndex];
        }

        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(true);
            portraitImage.sprite = branchPortraits[branchIndex];
        }

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        currentSentence = branchTexts[branchIndex];
        typingCoroutine = StartCoroutine(TypeText(currentSentence));
    }
    
    private IEnumerator ShowDialogueAfterFade(DialogueLine line)
    {
        yield return null;

        while (TutorialManager.Instance != null &&
            !TutorialManager.Instance.IsFading())
        {
            yield return null;
        }

        while (TutorialManager.Instance != null &&
            TutorialManager.Instance.IsFading())
        {
            yield return null;
        }

        ToggleDialogueUI(true);

        if (nameText != null)
        {
            if (string.IsNullOrEmpty(line.speaker))
            {
                nameText.gameObject.SetActive(false);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                nameText.text = line.speaker;
            }
        }

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.gameObject.SetActive(true);
            }
        }

        if (string.IsNullOrEmpty(line.text))
            yield break;

        currentSentence = line.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentSentence));
    }
} 