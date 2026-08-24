using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingClickHandler : MonoBehaviour, IPointerClickHandler
{
    public string targetMiniGameScene; // "MiniGame_01" 등
    public GameObject closedPopup;      // 공용 팝업 
    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIModalState.IsAnyModalOpen) return; // 가방/도감/업적 팝업 등 뭔가 열려있으면 건물 클릭 무시

        if (GameManager.Instance == null) return;
        bool lotteryUnlocked = GameManager.Instance.gameData.lotteryRoomUnlocked;
        if (lotteryUnlocked)
        {
            if (closedPopup != null)
                closedPopup.SetActive(true);
        }
        else
        {
            GameManager.Instance.EnterMiniGame(targetMiniGameScene);
        }
    }
}