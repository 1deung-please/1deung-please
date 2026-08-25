using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordBookPanel : MonoBehaviour
{
    [Header("데이터")]
    public AchievementListData achievementList;
    public EndingListData endingList;

    [Header("엔딩")]
    public List<Image> endingSlotImages;   // endingList.endings 순서와 1:1로 맞춰서 Inspector에서 연결
    public List<Button> endingSlotButtons; // 위 Image와 같은 순서, 같은 오브젝트에 있으면 됨

    [Header("엔딩 팝업")]
    public GameObject endingPopupPanel;
    public Image endingPopupBadgeImage;
    public TMP_Text endingPopupTitleText;
    public Button endingPopupReplayButton;

    [Header("업적 (Size 20, id 1~20 순서로 배경판 위에 미리 배치해둔 슬롯 연결)")]
    public List<Image> achievementSlotImages;   // 위치 계산 없이 에디터에서 미리 배치
    public List<Button> achievementSlotButtons; // 위와 같은 순서, 같은 오브젝트에 있으면 됨

    [Header("업적 팝업")]
    public GameObject achievementPopupPanel;
    public Image popupBackgroundImage; // 업적마다 다른 팝업 배경 이미지
    public TMP_Text popupTitleText;
    public TMP_Text popupDescriptionText;

    void OnEnable()
    {
        RefreshEndings();
        RefreshAchievements();
    }

    void RefreshEndings()
    {
        if (endingList == null || endingSlotImages == null) return;

        for (int i = 0; i < endingSlotImages.Count && i < endingList.endings.Count; i++)
        {
            EndingInfo info = endingList.endings[i];
            bool unlocked = AchievementStorage.IsUnlocked(info.linkedAchievementId);
            endingSlotImages[i].sprite = unlocked ? info.unlockedIcon : info.lockedIcon;
            endingSlotImages[i].alphaHitTestMinimumThreshold = 0.1f; // 투명한 부분은 클릭 무시

            if (endingSlotButtons != null && i < endingSlotButtons.Count && endingSlotButtons[i] != null)
            {
                Button btn = endingSlotButtons[i];
                btn.onClick.RemoveAllListeners();

                if (unlocked)
                {
                    btn.interactable = true;
                    var captured = info;
                    btn.onClick.AddListener(() => ShowEndingPopup(captured));
                }
                else
                {
                    // 잠긴 엔딩은 클릭해도 팝업 자체가 안 뜸
                    btn.interactable = false;
                }
            }
        }
    }

    void ShowEndingPopup(EndingInfo info)
    {
        if (endingPopupPanel == null) return;

        if (endingPopupBadgeImage != null) endingPopupBadgeImage.sprite = info.unlockedIcon;
        if (endingPopupTitleText != null) endingPopupTitleText.text = info.title;

        if (endingPopupReplayButton != null)
        {
            endingPopupReplayButton.onClick.RemoveAllListeners();
            endingPopupReplayButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadScene(info.sceneName);
            });
        }

        endingPopupPanel.SetActive(true);
    }

    public void CloseEndingPopup()
    {
        if (endingPopupPanel != null) endingPopupPanel.SetActive(false);
    }

    void RefreshAchievements()
    {
        if (achievementList == null || achievementSlotImages == null) return;

        // 일단 전부 잠금 상태(숨김)로 초기화 - 배경판이 이미 빈 상태를 보여주므로 안 켬
        for (int i = 0; i < achievementSlotImages.Count; i++)
        {
            if (achievementSlotImages[i] != null)
                achievementSlotImages[i].gameObject.SetActive(false);

            if (achievementSlotButtons != null && i < achievementSlotButtons.Count && achievementSlotButtons[i] != null)
                achievementSlotButtons[i].onClick.RemoveAllListeners();
        }

        foreach (var info in achievementList.achievements)
        {
            if (!AchievementStorage.IsUnlocked(info.id)) continue;

            int idx = info.id - 1; // id 1~20 -> 슬롯 인덱스 0~19
            if (idx < 0 || idx >= achievementSlotImages.Count) continue;

            if (achievementSlotImages[idx] != null)
            {
                achievementSlotImages[idx].gameObject.SetActive(true);
                achievementSlotImages[idx].sprite = info.badge;
                achievementSlotImages[idx].alphaHitTestMinimumThreshold = 0.1f; // 투명한 부분은 클릭 무시
            }

            if (achievementSlotButtons != null && idx < achievementSlotButtons.Count && achievementSlotButtons[idx] != null)
            {
                var captured = info; // 클로저 캡처
                achievementSlotButtons[idx].onClick.AddListener(() => ShowAchievementPopup(captured));
            }
        }
    }

    void ShowAchievementPopup(AchievementInfo info)
    {
        if (achievementPopupPanel == null) return;

        if (popupBackgroundImage != null) popupBackgroundImage.sprite = info.popupImage;
        if (popupTitleText != null) popupTitleText.text = info.title;
        if (popupDescriptionText != null) popupDescriptionText.text = info.description;

        achievementPopupPanel.SetActive(true);
    }

    public void CloseAchievementPopup()
    {
        if (achievementPopupPanel != null) achievementPopupPanel.SetActive(false);
    }
}