using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameData gameData; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnStartGame() 
    {
        if (gameData.tutorialDone)
            SceneLoader.Instance.LoadScene("Narrative"); // ÀÌ¹Ì Æ©Åä¸®¾ó ºÃÀ¸¸é ½ºÅµ
        else
            SceneLoader.Instance.LoadScene("Tutorial"); // Ã³À½ÀÌ¸é Æ©Åä¸®¾ó·Î
    }

    public void OnTutorialComplete()
    {
        gameData.tutorialDone = true;
        SceneLoader.Instance.LoadScene("Narrative");
    }

    public void OnMiniGameComplete(int miniGameIndex, int score)
    {
        switch (miniGameIndex)
        {
            case 1: gameData.miniGame1Score = score; break;
            case 2: gameData.miniGame2Score = score; break;
            case 3: gameData.miniGame3Score = score; break;
        }
    }

    public void DetermineEnding()
    {
        int total = gameData.TotalScore;

        if (total >= 61)
            SceneLoader.Instance.LoadScene("Ending_C");
        else if (total >= 31)
            SceneLoader.Instance.LoadScene("Ending_B");
        else
            SceneLoader.Instance.LoadScene("Ending_A");
    }
}
