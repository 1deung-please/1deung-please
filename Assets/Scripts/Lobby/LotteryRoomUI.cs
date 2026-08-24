using UnityEngine;

public class LotteryRoomUI : MonoBehaviour
{
    public GameObject bagIcon;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.gameData == null) return;

        bool unlocked = GameManager.Instance.gameData.lotteryRoomUnlocked;

        if (bagIcon != null)
            bagIcon.SetActive(!unlocked);
    }
}