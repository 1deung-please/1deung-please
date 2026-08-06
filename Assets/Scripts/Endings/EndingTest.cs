using UnityEngine;

public class EndingTest : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    [Header("테스트용")]
    public int score;
    public int[] playCount = new int[3];

    public void TestEnding()
    {
        // 점수 적용
        gameData.miniGame1Score = score;
        gameData.miniGame2Score = 0;
        gameData.miniGame3Score = 0;

        // 플레이 횟수 적용
        for (int i = 0; i < playCount.Length; i++)
        {
            gameData.playCount[i] = playCount[i];
        }

        // 엔딩 실행
        GetComponent<EndingManager>().DetermineEnding();
    }
}