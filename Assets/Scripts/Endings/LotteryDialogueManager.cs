using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LotteryDialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueData
    {
        public string speakerName;
        public string sentence;
        public GameObject activePortrait;
    }

    [Header("Panels")]
    public GameObject dialoguePanel;   // 전체 대화창 패널
    public GameObject scratchPanel;    // 복권 긁기 패널

    [Header("Dialogue Elements")]
    public TMP_Text nameText;           // NameText
    public TMP_Text dialogueText;       // DialogueText
    public GameObject portraitImage1;   // 주인공 초상화
    public GameObject portraitImage2;   // 종업원 초상화
    public GameObject dialogueUI;      // 대화 완료 시 출력되는 작은 화살표/삼각형 UI

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;

    private DialogueData[] dialogues;
    private int dialogueIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence = "";
    private bool clickRequested = false;

    private void Start()
    {
        if (scratchPanel != null)
            scratchPanel.SetActive(false);

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        InitDialogues();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(StartDialogueSequence());
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            clickRequested = true;
        }
    }

    private void InitDialogues()
    {
        dialogues = new DialogueData[]
        {
            new DialogueData
            {
                speakerName = "주인공",
                sentence = "숲이또 하나 주세요.",
                activePortrait = portraitImage1
            },
            new DialogueData
            {
                speakerName = "종업원",
                sentence = "네. 여깄습니다.",
                activePortrait = portraitImage2
            }
        };
    }

    private IEnumerator StartDialogueSequence()
    {
        for (dialogueIndex = 0; dialogueIndex < dialogues.Length; dialogueIndex++)
        {
            yield return Dialogue(dialogues[dialogueIndex]);
        }

        EndDialogue();
    }

    private IEnumerator Dialogue(DialogueData data)
    {
        clickRequested = false;

        // 대화 시작 시 클릭 아이콘 끄기
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        // 이름 설정
        if (nameText != null)
            nameText.text = data.speakerName;

        // 초상화 설정
        if (portraitImage1 != null) portraitImage1.SetActive(false);
        if (portraitImage2 != null) portraitImage2.SetActive(false);

        if (data.activePortrait != null)
            data.activePortrait.SetActive(true);

        // 타이핑 출력
        currentSentence = data.sentence;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentSentence));
        yield return typingCoroutine;

        // 타이핑이 끝나면 클릭 화살표 표시
        if (dialogueUI != null)
            dialogueUI.SetActive(true);

        clickRequested = false;

        // 마우스 클릭 기다림
        yield return new WaitUntil(() => clickRequested);
        clickRequested = false;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            // 타이핑 도중 클릭 시 즉시 완결
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
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (scratchPanel != null)
            scratchPanel.SetActive(true);
    }
}