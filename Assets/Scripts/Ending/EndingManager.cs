using UnityEngine;

public class EndingManager : MonoBehaviour
{
    private GameData gameData;

    [Header("테스트")]
    [SerializeField] private bool useTestData = false;
    [SerializeField] private GameData testGameData;

    public enum EndingType
    {
        ThinMotive,    // 얄팍한 속셈
        Unqualified,   // 자격 미달
        HalfSuccess,   // 절반의 성공
        TrueHero       // 진정한 귀인
    }

    private void Start()
    {
        if (useTestData)
        {
            gameData = testGameData;
            Debug.Log("테스트 GameData 사용");
            return;
        }

        gameData = GameManager.Instance.gameData;
    }

    public void DetermineEnding()
    {
        if (gameData == null)
        {
            Debug.LogWarning("GameData가 없습니다.");
            return;
        }

        int total = gameData.TotalScore;

        // 엔딩 1 : 얄팍한 속셈
        if (HasUnplayedGame())
        {
            SceneLoader.Instance.LoadScene("Ending_Shortcut");
            return;
        }

        // 엔딩 2 : 자격 미달
        if (total < 2000)
        {
            Debug.Log("엔딩 2 : 자격 미달");
            SceneLoader.Instance.LoadScene("Ending_A");
        }
        // 엔딩 3 : 절반의 성공
        else if (total < 8500)
        {
            Debug.Log("엔딩 3 : 절반의 성공");
            SceneLoader.Instance.LoadScene("Ending_B");
        }
        // 엔딩 4 : 진정한 귀인
        else
        {
            Debug.Log("엔딩 4 : 진정한 귀인");
            SceneLoader.Instance.LoadScene("Ending_C");
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
}