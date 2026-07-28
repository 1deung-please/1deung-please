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
    public bool[] playedGames = new bool[3];
    // 플레이 횟수
    public int[] playCount = new int[3];
    // 성공 여부(업적용)
    public bool[] successGames = new bool[3];
    // 실패 여부(업적용)
    public bool[] failGames = new bool[3];

    [Header("Achievement Tracking")]
    public int[] consecutiveSuccess = new int[3];
    public int[] consecutiveFail = new int[3];
    public int tutorialNoButtonCount;

    [Header("Merit Point")]
    public int meritPoint;

    [Header("Flags")]
    public bool tutorialDone;

    [Header("Global Timer")]
    public float globalTimeRemaining = 300f;
    public bool isTimerFrozen = true; // 튜토리얼 끝나기 전까지는 얼려둔 상태로 시작
    public bool lotteryRoomUnlocked = false;
    public bool isTimeOver = false;

    // 개발 중 초기화용
    public void ResetData()
    {
        miniGame1Score = 0;
        miniGame2Score = 0;
        miniGame3Score = 0;
        meritPoint = 0;
        tutorialDone = false;
        globalTimeRemaining = 300f;
        isTimerFrozen = true;
        lotteryRoomUnlocked = false;
        isTimeOver = false;
        playedGames = new bool[3];
        playCount = new int[3];
        successGames = new bool[3];
        failGames = new bool[3];
        consecutiveSuccess = new int[3];
        consecutiveFail = new int[3];
        tutorialNoButtonCount = 0;
    }
}