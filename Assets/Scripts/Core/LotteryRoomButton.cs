using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LotteryRoomButton : MonoBehaviour
{
    public void GoToLotteryRoom()
    {
        SceneManager.LoadScene("LotteryRoom");
    }
}
