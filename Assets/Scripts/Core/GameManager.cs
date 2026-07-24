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
            SceneLoader.Instance.LoadScene("Lobby"); // 이미 튜토리얼 봤으면 스킵
        else
            SceneLoader.Instance.LoadScene("Tutorial"); // 처음이면 튜토리얼로
    }

    public void OnTutorialComplete()
    {
        gameData.tutorialDone = true;
        gameData.isTimerFrozen = false; // 로비 진입과 함께 전역 타이머 시작
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

    public void ReturnToLobby()
    {
        isMiniGamePlaying = false;

        if (pendingEndingTransition)
        {
            // 전역 타이머가 이 미니게임 도중 이미 종료됐던 경우 → 복권방 활성화 상태로 로비 진입
            pendingEndingTransition = false;
            gameData.lotteryRoomUnlocked = true;
        }

        SceneLoader.Instance.LoadScene("Lobby");
    }

    void OnGlobalTimerEnd()
    {
        if (isMiniGamePlaying)
        {
            pendingEndingTransition = true; // 진행 중인 게임은 끝까지 인정, 종료 후 처리 예약
        }
        else
        {
            gameData.lotteryRoomUnlocked = true;
            SceneLoader.Instance.LoadScene("Lobby"); // 복권방 활성화된 로비로 즉시 이동
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