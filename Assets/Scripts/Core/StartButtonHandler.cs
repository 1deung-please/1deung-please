using UnityEngine;

public class StartButtonHandler : MonoBehaviour
{
    public void OnClickStart()
    {
        GameManager.Instance.OnStartGame();
    }
}