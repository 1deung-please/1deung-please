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
    private bool hasPendingAchievementCheck = false;
    private MiniGameKind pendingAchievementKind;
    private bool pendingAchievementSuccess;
    private string pendingEndingId = null;

    [Header("Exit Popup")]
    [SerializeField] private GameObject exitConfirmPopup;

    // PlayerPrefs 저장용 키 값 정의
    private const string KEY_TIME_REMAINING = "GlobalTimeRemaining";
    private const string KEY_TIMER_FROZEN = "IsTimerFrozen";
    private const string KEY_TIME_OVER = "IsTimeOver";
    private const string KEY_MERIT_POINT = "MeritPoint";
    private const string KEY_TUTORIAL_DONE = "TutorialDone";
    private const string KEY_TUTORIAL_SKIP_AVAILABLE = "TutorialSkipAvailable";
    private const string KEY_MG1_SCORE = "MiniGame1Score";
    private const string KEY_MG2_SCORE = "MiniGame2Score";
    private const string KEY_MG3_SCORE = "MiniGame3Score";
    private const string KEY_PLAY_CYCLE = "PlayCycle";

    public bool IsPendingEndingTransition() => pendingEndingTransition;

    public void SetPendingAchievementCheck(MiniGameKind kind, bool success)
    {
        hasPendingAchievementCheck = true;
        pendingAchievementKind = kind;
        pendingAchievementSuccess = success;
    }

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ExitPopupManager.Instance != null)
            {
                ExitPopupManager.Instance.ShowPopup();
            }

            return;
        }

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

        PlayerPrefs.SetInt(KEY_MG1_SCORE, gameData.miniGame1Score);
        PlayerPrefs.SetInt(KEY_MG2_SCORE, gameData.miniGame2Score);
        PlayerPrefs.SetInt(KEY_MG3_SCORE, gameData.miniGame3Score);

        PlayerPrefs.SetInt(KEY_PLAY_CYCLE, gameData.playCycle);

        for (int i = 0; i < gameData.playCount.Length; i++)
        {
            PlayerPrefs.SetInt($"PlayCount_{i}", gameData.playCount[i]);
        }
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

            gameData.miniGame1Score = PlayerPrefs.GetInt(KEY_MG1_SCORE, 0);
            gameData.miniGame2Score = PlayerPrefs.GetInt(KEY_MG2_SCORE, 0);
            gameData.miniGame3Score = PlayerPrefs.GetInt(KEY_MG3_SCORE, 0);

            gameData.playCycle = PlayerPrefs.GetInt(KEY_PLAY_CYCLE, 1);

            for (int i = 0; i < gameData.playCount.Length; i++)
            {
                gameData.playCount[i] = PlayerPrefs.GetInt($"PlayCount_{i}", 0);
            }
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

        PlayerPrefs.DeleteKey(KEY_MG1_SCORE);
        PlayerPrefs.DeleteKey(KEY_MG2_SCORE);
        PlayerPrefs.DeleteKey(KEY_MG3_SCORE);

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
        int earnedPoint = (collectedCount >= targetCount)
            ? (collectedCount + 50)
            : Mathf.RoundToInt(collectedCount * 0.5f);

        gameData.miniGame1Score += earnedPoint;

        addMeritPoint(earnedPoint);

        SaveGameData();
    }

    public void CompleteMiniGame2(int correctCount)
    {
        int earnedPoint = correctCount * 20;
        gameData.miniGame2Score += earnedPoint;

        addMeritPoint(earnedPoint);

        SaveGameData();
    }

    public void CompleteMiniGame3(bool isSuccess)
    {
        int earnedPoint = isSuccess ? 700 : 0;
        gameData.miniGame3Score += earnedPoint;

        addMeritPoint(earnedPoint);

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

            SaveGameData();

            // 업적/엔딩 팝업은 로딩 화면이 완전히 끝난 뒤(씬 전환 완료 후)에만 표시
            bool hadPendingAchievement = hasPendingAchievementCheck;
            hasPendingAchievementCheck = false;

            SceneLoader.Instance.LoadSceneWithLoadingScreen("NightLobby", () =>
            {
                if (hadPendingAchievement && AchievementManager.Instance != null)
                    AchievementManager.Instance.OnMiniGameResult(pendingAchievementKind, pendingAchievementSuccess);

                if (AchievementManager.Instance != null)
                    AchievementManager.Instance.OnGlobalTimerEnd();
            });
            return;
        }

        // 미니게임 결과에 따른 업적 체크는 로비 씬 전환이 완료된 뒤 처리
        bool hadPendingAchievementLobby = hasPendingAchievementCheck;
        hasPendingAchievementCheck = false;

        if (!gameData.isTimeOver)
        {
            ResumeTimer();
        }

        SaveGameData();
        SceneLoader.Instance.LoadScene("Lobby", () =>
        {
            if (hadPendingAchievementLobby && AchievementManager.Instance != null)
                AchievementManager.Instance.OnMiniGameResult(pendingAchievementKind, pendingAchievementSuccess);
        });
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
            SaveGameData();

            SceneLoader.Instance.LoadSceneWithLoadingScreen("NightLobby", () =>
            {
                if (AchievementManager.Instance != null)
                    AchievementManager.Instance.OnGlobalTimerEnd();
            });
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

        // 업적/엔딩 팝업은 이 엔딩 씬이 끝나고 메인메뉴로 돌아갈 때 표시 (ReturnToMainMenuFromEnding 참고)
        pendingEndingId = endingId;

        EndingStorage.Unlock(endingId);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance가 null입니다!");
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }

    // 각 엔딩 매니저(Shallow/Unqualified/HalfSuccess/TrueBenefactor 등)의
    // "메인메뉴로" 버튼에서 SceneManager.LoadScene("MainMenu") 대신 이 함수를 호출하면
    // 메인메뉴 전환이 끝난 뒤 업적/엔딩 팝업이 표시됨
    public void ReturnToMainMenuFromEnding()
    {
        string endingIdToConfirm = pendingEndingId;
        pendingEndingId = null;

        // 히든 엔딩 조건: 4개 엔딩(얄팍한속셈/자격미달/절반의성공/진정한귀인)을 모두 봤고
        // 아직 히든 엔딩을 안 봤으면, 메인메뉴로 가지 않고 곧바로 히든 엔딩으로 이동
        bool hiddenReady =
            AchievementStorage.IsUnlocked(14) && AchievementStorage.IsUnlocked(15)
            && AchievementStorage.IsUnlocked(16) && AchievementStorage.IsUnlocked(17)
            && !EndingStorage.IsUnlocked("히든");

        if (hiddenReady)
        {
            // 직전 엔딩(endingIdToConfirm)의 업적/엔딩 팝업은 히든 엔딩 씬 진입 후 처리
            // 히든 엔딩 자체의 팝업은 여기서 unlock 처리하고, 히든 씬을 나갈 때(메인메뉴 복귀 시) 표시
            EndingStorage.Unlock("히든");
            pendingEndingId = "히든";

            SceneLoader.Instance.LoadScene("Ending_Hidden", () =>
            {
                if (!string.IsNullOrEmpty(endingIdToConfirm) && AchievementManager.Instance != null)
                    AchievementManager.Instance.OnEndingConfirmed(endingIdToConfirm);
            });
            return;
        }

        // 엔딩(히든 포함)까지 다 봤으면 메인메뉴로 돌아가는 시점에 새 회차 시작
        // (가방의 "다시 도전하기"와 동일하게 전역 타이머/데이터를 리셋)
        ResetCycleData();

        SceneLoader.Instance.LoadScene("MainMenu", () =>
        {
            if (!string.IsNullOrEmpty(endingIdToConfirm) && AchievementManager.Instance != null)
                AchievementManager.Instance.OnEndingConfirmed(endingIdToConfirm);
        });
    }

    // 새 회차 시작을 위한 데이터 리셋만 수행 (씬 전환은 호출하는 쪽에서 처리)
    private void ResetCycleData()
    {
        bool playedOver3Min = gameData.globalTimeRemaining <= 120f;
        if (playedOver3Min)
        {
            PersistentStats.IncrementResetCycleCount();
        }

        gameData.playCycle++;

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
    }

    // 가방의 "다시 도전하기" 버튼 등에서 호출: 리셋 + 메인메뉴로 즉시 전환
    public void ResetCycle()
    {
        ResetCycleData();
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

        PlayerPrefs.DeleteKey(KEY_MG1_SCORE);
        PlayerPrefs.DeleteKey(KEY_MG2_SCORE);
        PlayerPrefs.DeleteKey(KEY_MG3_SCORE);

        PlayerPrefs.Save();
        Debug.Log("[GameManager] 세이브 데이터가 완전히 삭제되었습니다.");
    }
}