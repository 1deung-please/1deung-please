using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameData gameData;

    private const int MAX_MERIT_POINT = 10000;

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
    
    public void CompleteMiniGame1(int collectedCount, int targetCount)
    {
        gameData.miniGame1Score = collectedCount;

        if (collectedCount >= targetCount)
        {
            addMeritPoint(collectedCount + 50);
        }
        else
        {
            int merit = Mathf.RoundToInt(collectedCount * 0.5f);
            addMeritPoint(merit);
        }
    }

    public void CompleteMiniGame2(int correctCount)
    {
        gameData.miniGame2Score = correctCount;
        addMeritPoint(correctCount * 20);
    }

    public void CompleteMiniGame3(bool isSuccess)
    {
        if (isSuccess)
        {
            addMeritPoint(700);
        }
    }

    public void addMeritPoint(int amount)
    {
        gameData.meritPoint += amount;

        if (gameData.meritPoint > MAX_MERIT_POINT)
        {
            gameData.meritPoint = MAX_MERIT_POINT;
        }
    }

    public int getMeritPoint()
    {
        return gameData.meritPoint;
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
        string endingId;
        string sceneName;

        bool allPlayed = gameData.playedGames[0] && gameData.playedGames[1] && gameData.playedGames[2];

        if (!allPlayed)
        {
            // 얄팍한 속셈: 공덕 무관, 최우선 조건
            endingId = "얄팍한속셈";
            sceneName = "Ending_Shallow";
        }
        else
        {
            int total = gameData.meritPoint;

            if (total >= 8500)
            {
                endingId = "진정한귀인";
                sceneName = "Ending_TrueBenefactor";
            }
            else if (total >= 2000)
            {
                endingId = "절반의성공";
                sceneName = "Ending_HalfSuccess";
            }
            else
            {
                endingId = "자격미달";
                sceneName = "Ending_Unqualified";
            }
        }

        // 히든 엔딩: 나머지 4개를 이미 다 모았으면 공덕/조건 무관하게 즉시 히든으로
        if (AchievementStorage.IsUnlocked(14) && AchievementStorage.IsUnlocked(15)
            && AchievementStorage.IsUnlocked(16) && AchievementStorage.IsUnlocked(17))
        {
            endingId = "히든";
            sceneName = "Ending_Hidden";
        }

        AchievementManager.Instance.OnEndingConfirmed(endingId);
        SceneLoader.Instance.LoadScene(sceneName);
    }
}