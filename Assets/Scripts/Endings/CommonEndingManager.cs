using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommonEndingManager : MonoBehaviour
{
    [Header("Ending UI")]
    public TMP_Text titleText;
    public TMP_Text meritPointText;
    public TMP_Text pointText;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameData gameData = GameManager.Instance.gameData;

        // 제목
        titleText.text = "N번째.. 쌓은 공덕 포인트";

        // 최종 누적 공덕 포인트
        meritPointText.text = gameData.meritPoint + " pt";

        // 미니게임별 포인트
        pointText.text =
            "이걸 안 비켜? " + gameData.miniGame2Score + " PT\n" +
            "출격! 논리요새 " + gameData.miniGame3Score + " PT\n" +
            "주워줘, 쓰레기! " + gameData.miniGame1Score + " PT";
    }
}
