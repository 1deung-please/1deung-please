using UnityEngine;

public class ExitPopupManager : MonoBehaviour
{
    public static ExitPopupManager Instance;

    [SerializeField] private GameObject exitConfirmPopup;

    public bool IsPopupActive => exitConfirmPopup != null && exitConfirmPopup.activeSelf;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowPopup()
    {
        if (exitConfirmPopup != null && !exitConfirmPopup.activeSelf)
        {
            exitConfirmPopup.SetActive(true);
        }
    }

    public void HidePopup()
    {
        if (exitConfirmPopup != null)
        {
            exitConfirmPopup.SetActive(false);
        }

        // 종료 취소 시 타이머 다시 시작
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeTimer();
        }
    }

    public void ConfirmExit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGameData();
        }

        Application.Quit();
    }
}