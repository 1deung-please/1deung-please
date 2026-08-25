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

    public bool IsPendingEndingTransition() => pendingEndingTransition;

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
        if (gameData == null)
            return;

        if (gameData.isTimerFrozen)
            return;

        if (gameData.globalTimeRemaining <= 0)
            return;

        gameData.globalTimeRemaining -= Time.deltaTime;
        
        if (gameData.globalTimeRemaining <= 0)
        {
            gameData.globalTimeRemaining = 0;
            OnGlobalTimerEnd();
        }
    }

    public void PauseTimer()
    {
        gameData.isTimerFrozen = true;
    }

    public void ResumeTimer()
    {
        if (!gameData.isTimeOver)
            gameData.isTimerFrozen = false;
    }

    public void OnStartGame()
    {
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void OnTutorialComplete()
    {
        gameData.tutorialDone = true;
        gameData.isTimerFrozen = false;
     
        SceneLoader.Instance.LoadScene("Lobby");
    }

    public void EnterMiniGame(string miniGameSceneName)
    {
        isMiniGamePlaying = true;
        PauseTimer();
        SceneLoader.Instance.LoadScene(miniGameSceneName);
    }

    public void OnMiniGameStart()
    {
        ResumeTimer();
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
            if (played) count++;
        }
        return count;
    }

    public void CompleteMiniGame1(int collectedCount, int targetCount)
    {
        gameData.miniGame1Score = collectedCount;

        if (collectedCount >= targetCount)
            addMeritPoint(collectedCount + 50);
        else
            addMeritPoint(Mathf.RoundToInt(collectedCount * 0.5f));
    }

    public void CompleteMiniGame2(int correctCount)
    {
        gameData.miniGame2Score = correctCount;
        addMeritPoint(correctCount * 20);
    }

    public void CompleteMiniGame3(bool isSuccess)
    {
        gameData.miniGame3Score = isSuccess ? 700 : 0;
        if (isSuccess) addMeritPoint(700);
    }

    public void addMeritPoint(int amount)
    {
        gameData.meritPoint += amount;
        if (gameData.meritPoint > MAX_MERIT_POINT)
            gameData.meritPoint = MAX_MERIT_POINT;
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
            pendingEndingTransition = false;
            gameData.lotteryRoomUnlocked = true;
            AchievementManager.Instance.OnGlobalTimerEnd();

            SceneLoader.Instance.LoadScene("NightLobby");
            return;
        }

        if (!gameData.isTimeOver)
        {
            ResumeTimer();
        }

        SceneLoader.Instance.LoadScene("Lobby");
    }

    void OnGlobalTimerEnd()
    {
        gameData.isTimeOver = true;
        gameData.isTimerFrozen = true;

        if (isMiniGamePlaying)
        {
            pendingEndingTransition = true;
        }
        else
        {
            gameData.lotteryRoomUnlocked = true;
            AchievementManager.Instance.OnGlobalTimerEnd();
            SceneLoader.Instance.LoadScene("NightLobby");
        }
    }

    public void OnLotteryRoomClicked()
    {
        SceneLoader.Instance.LoadScene("LotteryRoom");
    }

    public void DetermineEnding()
    {
        if (gameData == null)
        {
            Debug.LogError("gameData가 null입니다. Test GameData를 연결하세요.");
            return;
        }

        string endingId;
        string sceneName;

        bool allPlayed = gameData.playedGames[0] && gameData.playedGames[1] && gameData.playedGames[2];

        // 기본 엔딩 결정
        if (!allPlayed)
        {
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

        // 히든 엔딩 조건
        if (AchievementStorage.IsUnlocked(14) && AchievementStorage.IsUnlocked(15) && AchievementStorage.IsUnlocked(16) && AchievementStorage.IsUnlocked(17))
        {
            endingId = "히든";
            sceneName = "Ending_Hidden";

            Debug.Log("히든 엔딩 조건 달성!");
        }

        Debug.Log("선택된 엔딩: " + endingId);
        Debug.Log("이동할 씬: " + sceneName);

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnEndingConfirmed(endingId);
        }

        EndingStorage.Unlock(endingId);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance가 null입니다!");
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }

    // ---- 사이클 초기화 (F-15) ----
    public void ResetCycle()
    {
        // 19번 업적 체크: 전역 5분 중 3분(180초) 이상 흘렀는지 = 남은 시간이 120초 이하였는지
        bool playedOver3Min = gameData.globalTimeRemaining <= 120f;
        if (playedOver3Min)
        {
            PersistentStats.IncrementResetCycleCount();
        }

        gameData.ResetData(); // 세션 데이터만 리셋 (엔딩/업적 해금 현황은 별도 PlayerPrefs라 영향 없음)

        SceneLoader.Instance.LoadScene("MainMenu"); // 시작화면 → 이후 튜토리얼(스킵 가능)
    }
}