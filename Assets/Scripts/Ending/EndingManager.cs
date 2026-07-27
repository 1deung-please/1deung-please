using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    public void FinishGame()
    {
        gameData.tutorialDone = true;
    }
}