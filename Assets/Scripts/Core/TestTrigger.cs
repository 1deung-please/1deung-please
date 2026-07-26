using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.OnStartGame(); // MainMenu → Tutorial
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.OnTutorialComplete(); // Tutorial → Lobby (전역 타이머 시작)
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.EnterMiniGame("MiniGame_01"); // Lobby → MiniGame_01
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.Instance.ReturnToLobby(); // MiniGame_01 → Lobby
        }

        // 아래는 전역 타이머 테스트 전용 (실제 게임 로직 아님)
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.gameData.globalTimeRemaining = 3f; // 5분 다 기다리기 귀찮으니 3초로 강제 단축
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("남은 시간: " + GameManager.Instance.gameData.globalTimeRemaining);
        }
    }
}