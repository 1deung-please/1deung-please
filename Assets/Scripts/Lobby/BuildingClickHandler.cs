using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickHandler : MonoBehaviour, IPointerClickHandler
{
    public string targetMiniGameScene; // "MiniGame_01" 등

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIModalState.IsAnyModalOpen) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.EnterMiniGame(targetMiniGameScene);
    }
}