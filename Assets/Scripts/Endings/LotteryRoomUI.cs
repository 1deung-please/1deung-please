using UnityEngine;
using UnityEngine.UI;

public class LotteryRoomUI : MonoBehaviour
{
    public GameObject lotteryRoomButton;
    public GameObject[] normalLobbyButtons; // 미니게임 입구들 + 가방 아이콘 포함

    void Update()
    {
        bool unlocked = GameManager.Instance.gameData.lotteryRoomUnlocked;

        lotteryRoomButton.SetActive(unlocked);

        foreach (var btn in normalLobbyButtons)
        {
            if (btn != null) btn.SetActive(!unlocked);
        }
    }

    public void OnClickLotteryRoom()
    {
        GameManager.Instance.OnLotteryRoomClicked();
    }
}