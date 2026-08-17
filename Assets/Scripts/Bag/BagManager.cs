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
    [SerializeField] private Button xButton;

    [Header("가방 효과음")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSfx;
    [SerializeField] private AudioClip closeSfx;
    [SerializeField] private AudioClip buttonSfx;

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

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

        if (xButton != null)
        {
            Image xImage = xButton.GetComponent<Image>();

            if (xImage != null)
            xImage.alphaHitTestMinimumThreshold = 0.1f;
        }
    }

    public void OpenBag()
    {
        PlaySfx(openSfx); 

        bagPanel.SetActive(true);
        menuPanel.SetActive(true);
        restartConfirmPanel.SetActive(false);
        Time.timeScale = 0f;
        endingBookPanel.SetActive(false);
        achievementBookPanel.SetActive(false);
    }

    public void CloseBag()
    {
        PlaySfx(closeSfx); 

        bagPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenRestartConfirm()
    {
        PlaySfx(buttonSfx);

        menuPanel.SetActive(false);
        restartConfirmPanel.SetActive(true);
    }

    public void CancelRestart()
    {
        PlaySfx(buttonSfx);

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
    PlaySfx(buttonSfx);

    menuPanel.SetActive(false);
    endingBookPanel.SetActive(true);
}

public void CloseEndingBook()
{
    PlaySfx(buttonSfx);

    endingBookPanel.SetActive(false);
    menuPanel.SetActive(true);
}

public void OpenAchievementBook()
{
    PlaySfx(buttonSfx);

    menuPanel.SetActive(false);
    achievementBookPanel.SetActive(true);
}

public void CloseAchievementBook()
{
    PlaySfx(buttonSfx);
    
    achievementBookPanel.SetActive(false);
    menuPanel.SetActive(true);
}
}
