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

    public int TotalScore => miniGame1Score + miniGame2Score + miniGame3Score;

    // 개발 중 초기화용 
    public void ResetData()
    {
        miniGame1Score = 0;
        miniGame2Score = 0;
        miniGame3Score = 0;
        tutorialDone = false;
    }
}
