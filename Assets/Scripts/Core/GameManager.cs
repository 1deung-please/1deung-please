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

    // PlayerPrefs 저장용 키 값 정의
    private const string KEY_TIME_REMAINING = "GlobalTimeRemaining";
    private const string KEY_TIMER_FROZEN = "IsTimerFrozen";
    private const string KEY_TIME_OVER = "IsTimeOver";
    private const string KEY_MERIT_POINT = "MeritPoint";
    private const string KEY_TUTORIAL_DONE = "TutorialDone";
    private const string KEY_TUTORIAL_SKIP_AVAILABLE = "TutorialSkipAvailable";

    public bool IsPendingEndingTransition() => pendingEndingTransition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 최초 실행 시 저장된 플레이 데이터 불러오기 
            LoadGameData();
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

    public void SaveGameData()
    {
        if (gameData == null) return;

        PlayerPrefs.SetFloat(KEY_TIME_REMAINING, gameData.globalTimeRemaining);
        PlayerPrefs.SetInt(KEY_TIMER_FROZEN, gameData.isTimerFrozen ? 1 : 0);
        PlayerPrefs.SetInt(KEY_TIME_OVER, gameData.isTimeOver ? 1 : 0);
        PlayerPrefs.SetInt(KEY_MERIT_POINT, gameData.meritPoint);
        PlayerPrefs.SetInt(KEY_TUTORIAL_DONE, gameData.tutorialDone ? 1 : 0);
        PlayerPrefs.SetInt(KEY_TUTORIAL_SKIP_AVAILABLE, gameData.tutorialSkipAvailable ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log($"[GameManager] 플레이 데이터 저장 완료 (남은 시간: {gameData.globalTimeRemaining:F1}초, 튜토리얼 완료: {gameData.tutorialDone})");
    }

    public void LoadGameData()
    {
        if (gameData == null) return;

        // 저장된 남은 시간 데이터가 존재하면 불러오기
        if (PlayerPrefs.HasKey(KEY_TIME_REMAINING))
        {
            gameData.globalTimeRemaining = PlayerPrefs.GetFloat(KEY_TIME_REMAINING);
            gameData.isTimerFrozen = PlayerPrefs.GetInt(KEY_TIMER_FROZEN, 0) == 1;
            gameData.isTimeOver = PlayerPrefs.GetInt(KEY_TIME_OVER, 0) == 1;
            gameData.meritPoint = PlayerPrefs.GetInt(KEY_MERIT_POINT, 0);
            gameData.tutorialDone = PlayerPrefs.GetInt(KEY_TUTORIAL_DONE, 0) == 1;
            gameData.tutorialSkipAvailable = PlayerPrefs.GetInt(KEY_TUTORIAL_SKIP_AVAILABLE, 0) == 1;

            Debug.Log($"[GameManager] 저장된 플레이 데이터 불러오기 완료 (남은 시간: {gameData.globalTimeRemaining:F1}초)");
        }
        else
        {
            // 저장된 기록이 없는 완전 첫 실행일 때만 리셋
            gameData.ResetData();
        }
    }

    private void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(KEY_TIME_REMAINING);
        PlayerPrefs.DeleteKey(KEY_TIMER_FROZEN);
        PlayerPrefs.DeleteKey(KEY_TIME_OVER);
        PlayerPrefs.DeleteKey(KEY_MERIT_POINT);
        PlayerPrefs.DeleteKey(KEY_TUTORIAL_DONE);
        PlayerPrefs.DeleteKey(KEY_TUTORIAL_SKIP_AVAILABLE);
        PlayerPrefs.Save();
        Debug.Log("[GameManager] 세이브 데이터 삭제 완료");
    }

    // 게임 종료 및 모바일 백그라운드 전환 이벤트
    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
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

    // 메인 메뉴 시작 버튼 호출 메서드
    public void OnStartGame()
    {
        if (gameData == null)
        return;

        // 전역 타이머가 이미 끝났으면 무조건 나이트 로비
        if (gameData.isTimeOver)
        {
            SceneLoader.Instance.LoadScene("NightLobby");
            return;
        }

        // 튜토리얼을 이미 완료/스킵했다면 로비
        if (gameData.tutorialDone)
        {
            ResumeTimer();
            SceneLoader.Instance.LoadScene("Lobby");
            return;
        }
        
        // 아직 튜토리얼을 완료하지 않았다면 튜토리얼
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void OnTutorialComplete()
    {
        gameData.tutorialDone = true;
        gameData.tutorialSkipAvailable = false;

        gameData.isTimerFrozen = false;
        gameData.isTimeOver = false;

        SaveGameData(); // 튜토리얼 완료 시점 저장
        SceneLoader.Instance.LoadScene("Lobby");
    }

    public void EnterMiniGame(string miniGameSceneName)
    {
        isMiniGamePlaying = true;
        PauseTimer();
        SaveGameData(); // 미니게임 진입 전 저장
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

        SaveGameData();
    }

    public void CompleteMiniGame2(int correctCount)
    {
        gameData.miniGame2Score = correctCount;
        addMeritPoint(correctCount * 20);
        SaveGameData();
    }

    public void CompleteMiniGame3(bool isSuccess)
    {
        gameData.miniGame3Score = isSuccess ? 700 : 0;
        if (isSuccess) addMeritPoint(700);
        SaveGameData();
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

            SaveGameData();
            SceneLoader.Instance.LoadSceneWithLoadingScreen("NightLobby");
            return;
        }

        if (!gameData.isTimeOver)
        {
            ResumeTimer();
        }

        SaveGameData();
        SceneLoader.Instance.LoadScene("Lobby");
    }

    void OnGlobalTimerEnd()
    {
        gameData.globalTimeRemaining = 0f;
        gameData.isTimeOver = true;
        gameData.isTimerFrozen = true;

        SaveGameData();

        if (isMiniGamePlaying)
        {
            pendingEndingTransition = true;
        }
        else
        {
            gameData.lotteryRoomUnlocked = true;
            AchievementManager.Instance.OnGlobalTimerEnd();
            SaveGameData();
            SceneLoader.Instance.LoadSceneWithLoadingScreen("NightLobby");
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

    public void ResetCycle()
    {
        bool playedOver3Min = gameData.globalTimeRemaining <= 120f;
        if (playedOver3Min)
        {
            PersistentStats.IncrementResetCycleCount();
        }

        // 새 회차 시작 시 기기 내부 세이브 파일 초기화
        ClearSavedData();

        gameData.ResetData(); // 세션 데이터 초기화

        // 회차 리셋 시에도 튜토리얼을 스킵하도록 true 처리 후 저장
        gameData.tutorialDone = false;
        gameData.tutorialSkipAvailable = true;

        gameData.globalTimeRemaining = 300f;
        gameData.isTimerFrozen = true;
        gameData.isTimeOver = false;

        SaveGameData();

        SceneLoader.Instance.LoadScene("MainMenu");
    }

    [ContextMenu("Clear PlayerPrefs Data")]
    public void ClearSavedDataPublic()
    {
        PlayerPrefs.DeleteKey(KEY_TIME_REMAINING);
        PlayerPrefs.DeleteKey(KEY_TIMER_FROZEN);
        PlayerPrefs.DeleteKey(KEY_TIME_OVER);
        PlayerPrefs.DeleteKey(KEY_MERIT_POINT);
        PlayerPrefs.DeleteKey(KEY_TUTORIAL_DONE);
        PlayerPrefs.DeleteKey(KEY_TUTORIAL_SKIP_AVAILABLE);
        PlayerPrefs.Save();
        Debug.Log("[GameManager] 세이브 데이터가 완전히 삭제되었습니다.");
    }
}