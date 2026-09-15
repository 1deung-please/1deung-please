using UnityEngine;
using UnityEngine.EventSystems;

public class NightBuildingClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject closedPopup; // 닫혀있다 팝업

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIModalState.IsAnyModalOpen) return;
        if (GameManager.Instance == null) return;

        if (closedPopup != null)
            closedPopup.SetActive(true);
    }
}