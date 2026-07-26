using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameData gameData;

    private bool isMiniGamePlaying = false;
    private bool pendingEndingTransition = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameData.ResetData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameData.isTimerFrozen) return;

        if (gameData.globalTimeRemaining > 0)
        {
            gameData.globalTimeRemaining -= Time.deltaTime;

            if (gameData.globalTimeRemaining <= 0)
            {
                gameData.globalTimeRemaining = 0;
                OnGlobalTimerEnd();
            }
        }
    }

    public void OnStartGame()
    {
        if (gameData.tutorialDone)
            SceneLoader.Instance.LoadScene("Lobby"); // �̹� Ʃ�丮�� ������ ��ŵ
        else
            SceneLoader.Instance.LoadScene("Tutorial"); // ó���̸� Ʃ�丮���
    }

    public void OnTutorialComplete()
    {
        gameData.tutorialDone = true;
        gameData.isTimerFrozen = false; // �κ� ���԰� �Բ� ���� Ÿ�̸� ����
        SceneLoader.Instance.LoadScene("Lobby");
    }

    public void EnterMiniGame(string miniGameSceneName)
    {
        isMiniGamePlaying = true;
        SceneLoader.Instance.LoadScene(miniGameSceneName);
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

    public void RecordMiniGamePlay(int miniGameIndex)
    {
        gameData.playedGames[miniGameIndex - 1] = true;
        gameData.playCount[miniGameIndex - 1]++;
    }

    public void RecordMiniGameResult(int miniGameIndex, bool success)
    {
        if (success)
            gameData.successGames[miniGameIndex - 1] = true;
        else
            gameData.failGames[miniGameIndex - 1] = true;
    }

    public int GetPlayedGameCount()
    {
        int count = 0;

        foreach (bool played in gameData.playedGames)
        {
            if (played)
                count++;
        }

        return count;
    }
    
    public void ReturnToLobby()
    {
        isMiniGamePlaying = false;

        if (pendingEndingTransition)
        {
            // ���� Ÿ�̸Ӱ� �� �̴ϰ��� ���� �̹� ����ƴ� ��� �� ���ǹ� Ȱ��ȭ ���·� �κ� ����
            pendingEndingTransition = false;
            gameData.lotteryRoomUnlocked = true;
        }

        SceneLoader.Instance.LoadScene("Lobby");
    }

    void OnGlobalTimerEnd()
    {
        if (isMiniGamePlaying)
        {
            pendingEndingTransition = true; // ���� ���� ������ ������ ����, ���� �� ó�� ����
        }
        else
        {
            gameData.lotteryRoomUnlocked = true;
            SceneLoader.Instance.LoadScene("Lobby"); // ���ǹ� Ȱ��ȭ�� �κ�� ��� �̵�
        }
    }

    public void OnLotteryRoomClicked()
    {
        DetermineEnding();
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