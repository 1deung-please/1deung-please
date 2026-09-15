using System.Collections;
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
    }

    public void HideChoice()
    {
        choicePanel.SetActive(false);
    }

    private void OnYesClicked()
    {
        HideChoice(); // 버튼 즉시 숨김
        noCount = 0;

        dialogueManager.StartBranchDialogue(
            new string[]
            {
                "(그래요, 한 번 믿어봅시다.)",
                "탁월한 선택입니다! 역시 귀인님은 그릇부터가 다르시네!",
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

        HideChoice(); // 버튼 즉시 숨김, 텍스트만 보이도록

        if (noCount >= 10)
        {
            StartCoroutine(TenthNoRoutine());
            return;
        }

        string begging = "";
        for (int i = 0; i < noCount; i++)
        {
            begging += "제발 ";
        }

        dialogueManager.RepeatCurrentDialogue(
            "도믿걸",
            domitGirlPortrait,
            begging + "운명 한 번 맡겨보시겠어요?");
        // ↑ 타이핑이 끝나도 바로 버튼이 뜨지 않고, 클릭을 한 번 더 기다린 뒤 버튼만 노출됨

        if (dialoguePanel != null)
        {
            Vector2 pos = dialoguePanel.anchoredPosition;
            pos.x = dialogueX;
            dialoguePanel.anchoredPosition = pos;
        }
    }

    // 10번째 NO: 암전 -> 업적 unlock/팝업 -> 팝업이 닫힐 때까지 대기 -> Fade In -> YES만 노출
    private IEnumerator TenthNoRoutine()
    {
        // 1. 화면 암전 (Fade In 없이 검은 상태 유지)
        // IsFading()은 alpha>0.1f일 때도 true를 리턴하므로 (암전 완료 상태 포함) 완료 대기에 쓸 수 없음 -> 정해진 시간만큼 대기
        TutorialManager.Instance.FadeOut();
        yield return new WaitForSeconds(TutorialManager.Instance.FadeDuration);

        // 2. 업적 unlock -> AchievementManager 큐에 팝업이 등록되고 즉시 표시됨
        dialogueManager.OnChoiceResult(false, noCount);

        // 팝업이 뜰 때까지 한 프레임 대기 (Enqueue는 동기 처리되지만 안전하게)
        yield return null;

        // 3. 업적 팝업이 사용자 클릭으로 닫힐 때까지 대기
        while (AchievementManager.Instance != null && AchievementManager.Instance.IsPopupActive)
            yield return null;

        // 4. 암전 해제
        TutorialManager.Instance.FadeIn();
        yield return new WaitForSeconds(TutorialManager.Instance.FadeDuration);

        // 5. NO 버튼 없이 YES 버튼만 노출
        ShowChoice();

        if (noButton != null)
            noButton.gameObject.SetActive(false);

        if (noButtonText != null)
            noButtonText.gameObject.SetActive(false);
    }

    public void ResetChoice()
    {
        noCount = 0;
        HideChoice();

        if (noButton != null)
            noButton.gameObject.SetActive(true);

        if (noButtonText != null)
            noButtonText.gameObject.SetActive(true);
    }

    public int GetNoCount()
    {
        return noCount;
    }
}