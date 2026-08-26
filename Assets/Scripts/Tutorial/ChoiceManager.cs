using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Sprite domitGirlPortrait;
    [SerializeField] private Sprite playerPortrait;


    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI noButtonText;

    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private RectTransform dialoguePanel;
    [SerializeField] private float dialogueX = -39;

    private int noCount = 0;

    private void Awake()
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        choicePanel.SetActive(false);
    }

    public void ShowChoice()
    {
        choicePanel.SetActive(true);

        if (noButtonText != null)
            noButtonText.text = "NO";
    }

    public void HideChoice()
    {
        choicePanel.SetActive(false);
    }

    private void OnYesClicked()
    {
        HideChoice();

        noCount = 0;

        dialogueManager.StartBranchDialogue(
            new string[]
            {
                "(그래요, 한 번 믿어봅시다.)",
                "(환하게 웃으며) 탁월한 선택입니다! 역시 귀인님은 그릇부터가 다르시네!",
                "자, 그럼 지금부터 조상님과 '동기화'되는 법을 알려드릴게요.",
                "우선 기초적인 덕부터 쌓아볼까요?"
            },
            new string[]
            {
                "주인공",
                "도믿걸",
                "도믿걸",
                "도믿걸"
            },
            new Sprite[]
            {
                playerPortrait,
                domitGirlPortrait,
                domitGirlPortrait,
                domitGirlPortrait
            }
        );
    }

    private void OnNoClicked()
    {
        noCount++;

        if (noCount >= 10)
        {
            HideChoice();

            TutorialManager.Instance.FadeOut();

            dialogueManager.OnChoiceResult(false, noCount);

            return;
        }
        string begging = "";

        for (int i = 0; i < noCount; i++)
        {
            begging += "제발 ";
        }

        dialogueManager.RepeatCurrentDialogue("도믿걸", domitGirlPortrait, begging + "운명 한 번 맡겨보시겠어요?");

        Vector2 pos = dialoguePanel.anchoredPosition;
        pos.x = dialogueX;
        dialoguePanel.anchoredPosition = pos;
    }

    public void ResetChoice()
    {
        noCount = 0;

        HideChoice();
    }

    public int GetNoCount()
    {
        return noCount;
    }
}