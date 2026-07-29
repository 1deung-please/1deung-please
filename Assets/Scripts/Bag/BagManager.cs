using UnityEngine;
using UnityEngine.SceneManagement;

public class BagManager : MonoBehaviour
{
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject restartConfirmPanel;
    [SerializeField] private GameObject endingBookPanel;
    [SerializeField] private GameObject achievementBookPanel;

    [Header("다시 시작 후 이동할 씬")]
    [SerializeField] private string startSceneName = "Lobby";

    private void Start()
    {
        bagPanel.SetActive(false);
        menuPanel.SetActive(true);
        restartConfirmPanel.SetActive(false);
        endingBookPanel.SetActive(false);
        achievementBookPanel.SetActive(false);
    }

    public void OpenBag()
    {
        bagPanel.SetActive(true);
        menuPanel.SetActive(true);
        restartConfirmPanel.SetActive(false);
        Time.timeScale = 0f;
        endingBookPanel.SetActive(false);
        achievementBookPanel.SetActive(false);
    }

    public void CloseBag()
    {
        bagPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenRestartConfirm()
    {
        menuPanel.SetActive(false);
        restartConfirmPanel.SetActive(true);
    }

    public void CancelRestart()
    {
        restartConfirmPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ConfirmRestart()
{
    Time.timeScale = 1f;

    if (GameManager.Instance == null)
    {
        Debug.LogError("GameManager.Instance가 없습니다.");
        return;
    }

    GameManager.Instance.ResetCycle();
}

public void OpenEndingBook()
{
    menuPanel.SetActive(false);
    endingBookPanel.SetActive(true);
}

public void CloseEndingBook()
{
    endingBookPanel.SetActive(false);
    menuPanel.SetActive(true);
}

public void OpenAchievementBook()
{
    menuPanel.SetActive(false);
    achievementBookPanel.SetActive(true);
}

public void CloseAchievementBook()
{
    achievementBookPanel.SetActive(false);
    menuPanel.SetActive(true);
}
}