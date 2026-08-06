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


    public enum EndingType
    {
        Ending_Shallow,    // 얄팍한 속셈
        Ending_Unqualified,   // 자격 미달
        Ending_HalfSuccess,   // 절반의 성공
        Ending_TrueBenefactor   // 진정한 귀인
    }


    private void Start()
    {
        Debug.Log("Start 실행");

        if (useTestData)
        {
            gameData = testGameData;
            Debug.Log("테스트 GameData 사용");
        }
        else
        {
            gameData = GameManager.Instance.gameData;
        }

        Debug.Log("gameData = " + gameData);
    }

    public void DetermineEnding()
    {
        Debug.Log("DetermineEnding");
        Debug.Log("gameData = " + gameData);

        if (gameData == null)
        {
            Debug.LogWarning("GameData가 없습니다.");
            return;
        }


        // 플레이하지 않은 게임이 있으면 최우선 엔딩
        if (HasUnplayedGame())
        {
            Debug.Log("엔딩 : 얄팍한 속셈");
            SceneManager.LoadScene("Ending_Shallow");
            return;
        }


        int total = gameData.meritPoint;


        // 자격 미달
        if (total < 2000)
        {
            Debug.Log("엔딩 : 자격 미달");
            SceneManager.LoadScene("Ending_Unqualified");
        }

        // 절반의 성공
        else if (total < 8500)
        {
            Debug.Log("엔딩 : 절반의 성공");
            SceneManager.LoadScene("Ending_HalfSuccess");
        }

        // 진정한 귀인
        else
        {
            Debug.Log("엔딩 : 진정한 귀인");
            SceneManager.LoadScene("Ending_TrueBenefactor");
        }
    }


    private bool HasUnplayedGame()
    {
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


    // 엔딩 대화 실행
    public void StartEndingDialogue()
    {
        dialogueManager.StartDialogue(endingDialogue);
    }


    // 게임 종료 처리
    public void FinishGame()
    {
        // 게임 종료 후 처리
        // 예: 메인 메뉴 이동, 버튼 활성화 등
    }
}