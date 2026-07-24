using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [Header("MiniGame Scores")]
    public int miniGame1Score;
    public int miniGame2Score;
    public int miniGame3Score;

    [Header("Flags")]
    public bool tutorialDone;

    [Header("Global Timer")]
    public float globalTimeRemaining = 300f;
    public bool isTimerFrozen = true; // 튜토리얼 끝나기 전까지는 얼려둔 상태로 시작
    public bool lotteryRoomUnlocked = false;

    public int TotalScore => miniGame1Score + miniGame2Score + miniGame3Score;

    // 개발 중 초기화용
    public void ResetData()
    {
        miniGame1Score = 0;
        miniGame2Score = 0;
        miniGame3Score = 0;
        tutorialDone = false;
        globalTimeRemaining = 300f;
        isTimerFrozen = true;
        lotteryRoomUnlocked = false;
    }
}