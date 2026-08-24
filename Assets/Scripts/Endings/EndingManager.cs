using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData endingDialogue;

    private GameData gameData;

    [Header("테스트")]
    [SerializeField] private bool useTestData = false;
    [SerializeField] private GameData testGameData;

    [Header("Scratch Button")]
    [SerializeField] private ScratchButtonManager scratchButtonManager;

    public enum EndingType
    {
        Ending_Shallow,       // 얄팍한 속셈
        Ending_Unqualified,   // 자격 미달
        Ending_HalfSuccess,   // 절반의 성공
        Ending_TrueBenefactor // 진정한 귀인
    }

    private void Awake()
    {
        Debug.Log("EndingManager Awake 실행");

        if (useTestData)
        {
            gameData = testGameData;
            Debug.Log("테스트 GameData 사용");
        }
        else if (GameManager.Instance != null)
        {
            gameData = GameManager.Instance.gameData;
        }
        else
        {
            Debug.LogError("GameManager.Instance가 null입니다! GameManager를 확인하세요.");
        }

        Debug.Log("gameData = " + gameData);
    }

    private void Start()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueFinished += ShowScratchButton;
        }
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

        bool allPlayed =
            gameData.playedGames[0] &&
            gameData.playedGames[1] &&
            gameData.playedGames[2];

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
        if (AchievementStorage.IsUnlocked(14)
            && AchievementStorage.IsUnlocked(15)
            && AchievementStorage.IsUnlocked(16)
            && AchievementStorage.IsUnlocked(17))
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

    private bool HasUnplayedGame()
    {
        if (gameData == null || gameData.playCount == null)
        {
            Debug.LogWarning("gameData가 null이어서 HasUnplayedGame을 체크할 수 없습니다.");
            return false;
        }

        foreach (int count in gameData.playCount)
        {
            if (count == 0)
            {
                Debug.Log("플레이하지 않은 게임이 있습니다.");
                return true;
            }
        }

        Debug.Log("모든 게임을 플레이했습니다.");
        return false;
    }

    public void StartEndingDialogue()
    {
        if (dialogueManager != null && endingDialogue != null)
        {
            dialogueManager.StartDialogue(endingDialogue);
        }
    }

    private bool scratchShown = false;

    private void ShowScratchButton()
    {
        if (scratchShown)
            return;

        scratchShown = true;

        Debug.Log("대화 종료 → 스크래치 버튼 등장");

        if (scratchButtonManager != null)
        {
            scratchButtonManager.ShowButton();
        }
    }

    private void OnDestroy()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueFinished -= ShowScratchButton;
        }
    }
}