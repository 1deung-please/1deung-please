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

    [Header("Flags")]
    public bool tutorialDone;

    [Header("Global Timer")]
    public float globalTimeRemaining = 300f;
    public bool isTimerFrozen = true; // Ʃ�丮�� ������ �������� ����� ���·� ����
    public bool lotteryRoomUnlocked = false;

    public int TotalScore => miniGame1Score + miniGame2Score + miniGame3Score;

    // ���� �� �ʱ�ȭ��
    public void ResetData()
    {
        miniGame1Score = 0;
        miniGame2Score = 0;
        miniGame3Score = 0;

        tutorialDone = false;

        globalTimeRemaining = 300f;
        isTimerFrozen = true;
        lotteryRoomUnlocked = false;

        playedGames = new bool[3];
        playCount = new int[3];
        successGames = new bool[3];
        failGames = new bool[3];
    }
}