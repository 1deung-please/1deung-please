using UnityEngine;

public class LobbyGameButtons : MonoBehaviour
{
    public void GoToMiniGame1()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.EnterMiniGame("MiniGame_01");
    }

    public void GoToMiniGame2()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.EnterMiniGame("MiniGame_02");
    }

    public void GoToMiniGame3()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.EnterMiniGame("MiniGame_03");
    }
}