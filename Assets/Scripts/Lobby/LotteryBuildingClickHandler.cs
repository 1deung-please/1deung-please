using UnityEngine;
using UnityEngine.EventSystems;

public class LotteryBuildingClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.gameData == null) return;

        bool unlocked = GameManager.Instance.gameData.lotteryRoomUnlocked;

        if (!unlocked) return; // 잠겨있으면 아무 반응 없음 (팝업도 안 뜸)

        GameManager.Instance.OnLotteryRoomClicked();
    }
}