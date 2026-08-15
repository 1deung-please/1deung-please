using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject restartConfirmPanel;
    [SerializeField] private GameObject endingBookPanel;
    [SerializeField] private GameObject achievementBookPanel;

    [Header("다시 시작 후 이동할 씬")]
    [SerializeField] private string startSceneName = "Lobby";

    [Header("가방 버튼")]
    [SerializeField] private Button bagButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        bagPanel.SetActive(false);
        menuPanel.SetActive(true);
        restartConfirmPanel.SetActive(false);
        endingBookPanel.SetActive(false);
        achievementBookPanel.SetActive(false);

        if (bagButton != null)
        {
            Image bagImage = bagButton.GetComponent<Image>();

            if (bagImage != null)
                bagImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (closeButton != null)
        {
            Image closeImage = closeButton.GetComponent<Image>();

            if (closeImage != null)
                closeImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (restartButton != null)
        {
            Image restartImage = restartButton.GetComponent<Image>();

            if (restartImage != null)
                restartImage.alphaHitTestMinimumThreshold = 0.1f;
        }
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
