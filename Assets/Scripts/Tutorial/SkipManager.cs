using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private GameObject skipButton;

    private void Start()
    {
        if (!gameData.tutorialDone)
        {
            skipButton.SetActive(false);
        }
        else
        {
            skipButton.SetActive(true);
        }
    }

    public void Skip()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void FinishFirstPlay()
    {
        gameData.tutorialDone = true;
    }
}