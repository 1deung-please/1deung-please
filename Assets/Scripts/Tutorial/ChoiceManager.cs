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

    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI noButtonText;

    [SerializeField] private DialogueManager dialogueManager;
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

        dialogueManager.OnChoiceResult(true);
    }

    private void OnNoClicked()
    {
        noCount++;

        if (noCount >= 10)
        {
            HideChoice();

            TutorialManager.Instance.FadeOut();

            return;
        }

        string begging = "";

        for (int i = 0; i < noCount; i++)
        {
            begging += "제발 ";
        }

        dialogueManager.RepeatCurrentDialogue("도믿걸", domitGirlPortrait, begging + "운명 한 번 맡겨보시겠어요?");
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