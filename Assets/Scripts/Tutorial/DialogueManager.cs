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
    [SerializeField] private ScratchLotteryManager scratchLotteryManager;

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

    // 타이핑이 끝났을 때 호출할 콜백
    private System.Action onTypingComplete;

    // NO 재입력 텍스트가 다 타이핑된 후, "다음 클릭"에 버튼을 보여줄지 대기하는 상태
    private bool awaitingChoiceReveal = false;

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
        awaitingChoiceReveal = false;

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (portraitImage != null)
            portraitImage.gameObject.SetActive(false);

        HideChoicePanel();

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.IsFading())
                return;

            if (TutorialManager.Instance.ConsumeClick())
                return;
        }

        // 선택지 버튼이 떠 있을 때는 대화 진행 금지 (버튼 클릭으로만 진행)
        if (choicePanel != null && choicePanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!dialogueStarted)
            {
                dialogueStarted = true;
                ShowDialogue();
                return;
            }

            // 타이핑 중이면 한 번 클릭해서 즉시 완성
            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                dialogueText.text = currentSentence;
                FinishTyping();
                return;
            }

            // ★ 재입력 텍스트가 다 타이핑되고 클릭을 기다리는 상태
            if (awaitingChoiceReveal)
            {
                awaitingChoiceReveal = false;

                // 텍스트 숨기고 버튼만 노출
                ToggleDialogueUI(false);
                ShowChoicePanel();
                return;
            }

            // 다음 대사로 넘어가기 전 UI 숨김
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

    private void ShowDialogue()
    {
        if (dialogueData == null ||
            dialogueData.lines == null ||
            currentIndex >= dialogueData.lines.Count)
        {
            EndDialogue();
            return;
        }

        currentLine = dialogueData.lines[currentIndex];

        DialogueLine line = currentLine;

        bool isBackgroundChange = IsBackgroundChangeLine(line);

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.SetDialoguePos(currentLine.isNormalDialogue);
        }

        // 이름
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

        // 초상화
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
                    portraitImage.gameObject.SetActive(true);
                else
                    portraitImage.gameObject.SetActive(false);
            }
        }

        // 우선 Dialogue UI / 선택지 버튼 숨김
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        HideChoicePanel();
        awaitingChoiceReveal = false;

        currentSentence = line.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // ★ 배경 전환 줄
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
            // 일반 대사: 이 줄이 선택지 줄이면, 타이핑 끝나자마자 텍스트와 "함께" 버튼 노출
            onTypingComplete = line.isChoice ? (System.Action)ShowChoicePanel : null;

            typingCoroutine = StartCoroutine(TypeText(currentSentence));

            ExecuteEvent(line.dialogueEvent);
        }

        ExecuteAnimEvent(line.animEvent);
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char c in text)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            yield return new WaitForSeconds(typingSpeed);
        }

        FinishTyping();
    }

    // 타이핑이 끝났을 때(자동 완료든, 클릭으로 스킵했든) 공통 처리
    private void FinishTyping()
    {
        isTyping = false;

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(true);

        if (dialogueUI != null &&
            currentLine != null &&
            !IsBackgroundChangeLine(currentLine) &&
            !string.IsNullOrEmpty(currentLine.speaker))
        {
            dialogueUI.SetActive(true);
        }

        var callback = onTypingComplete;
        onTypingComplete = null;
        callback?.Invoke();
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

        if (scratchLotteryManager != null)
        {
            scratchLotteryManager.ShowLottery();
            return;
        }

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

            case DialogueEvent.ChangeToTutorial_bus_stop:
                BackgroundManager.Instance.ChangeToTutorial_bus_stop();
                break;

            case DialogueEvent.ChangeToTutorial_underground_shopping_center:
                BackgroundManager.Instance.ChangeToTutorial_underground_shopping_center();
                break;

            case DialogueEvent.ChangeToTutorial_cafe:
                BackgroundManager.Instance.ChangeToTutorial_cafe();
                break;

            case DialogueEvent.ChangeToTutorial_bag:
                BackgroundManager.Instance.ChangeToTutorial_bag();
                break;

            case DialogueEvent.ChangeToStreet_Normal:
                BackgroundManager.Instance.ChangeToStreet_Normal();
                break;
        }
    }

    private bool IsBackgroundChangeLine(DialogueLine line)
    {
        if (line == null)
            return false;

        return line.dialogueEvent == DialogueEvent.ChangeStreet ||
               line.dialogueEvent == DialogueEvent.ChangeCafe;
    }

    private void ToggleDialogueUI(bool show)
    {
        if (nameText != null)
            nameText.gameObject.SetActive(show);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(show);

        if (portraitImage != null)
        {
            if (!show)
            {
                portraitImage.gameObject.SetActive(false);
            }
            else
            {
                if (currentLine != null &&
                    (currentLine.portrait != null ||
                     currentLine.animEvent != DialogueAnimEvent.None))
                {
                    portraitImage.gameObject.SetActive(true);
                }
            }
        }

        if (dialogueUI != null)
            dialogueUI.SetActive(show);
    }

    private void ExecuteAnimEvent(DialogueAnimEvent animEvent)
    {
        if (characterAnimator == null)
            return;

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
        {
            NextDialogue();
        }
        else
        {
            AchievementManager.Instance.OnTutorialNoButtonClicked();
        }
    }

    // ChoiceManager가 NO 클릭 시 호출: 텍스트만 다시 타이핑, 끝나면 "클릭 대기" 상태로 전환
    public void RepeatCurrentDialogue(
        string speaker,
        Sprite portrait,
        string newText)
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

        // ★ 일반 대사와 동일하게: 타이핑 시작 전엔 UI 숨김
        //   (기존엔 여기서 dialogueUI.SetActive(true)로 미리 보여주고 있었음)
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        awaitingChoiceReveal = false;

        // 타이핑 끝나면 FinishTyping()이 dialogueUI를 다시 보여주고,
        // 그 다음 클릭을 기다리는 상태로 전환
        onTypingComplete = () => { awaitingChoiceReveal = true; };

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    public void SkipDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

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

            NextDialogue();
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

        onTypingComplete = null;

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

        if (IsBackgroundChangeLine(line))
        {
            ToggleDialogueUI(false);

            if (dialogueText != null)
                dialogueText.text = "";

            if (nameText != null)
                nameText.gameObject.SetActive(false);

            if (portraitImage != null)
                portraitImage.gameObject.SetActive(false);

            currentIndex++;
            ShowDialogue();

            yield break;
        }

        ToggleDialogueUI(true);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(true);

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

        onTypingComplete = line.isChoice ? (System.Action)ShowChoicePanel : null;

        typingCoroutine = StartCoroutine(TypeText(currentSentence));
    }

    // ── 선택지 패널 표시/숨김 헬퍼 (ChoiceManager를 우선 사용) ──
    private void ShowChoicePanel()
    {
        if (choiceManager != null)
            choiceManager.ShowChoice();
        else if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    private void HideChoicePanel()
    {
        if (choiceManager != null)
            choiceManager.HideChoice();
        else if (choicePanel != null)
            choicePanel.SetActive(false);
    }
}