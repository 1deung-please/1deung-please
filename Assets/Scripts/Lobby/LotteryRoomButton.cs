using UnityEngine;
using UnityEngine.UI;

public class LotteryRoomButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.OnLotteryRoomClicked();
        });
    }
}