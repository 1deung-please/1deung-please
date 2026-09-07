using UnityEngine;

public class ExitPopupManager : MonoBehaviour
{
    public static ExitPopupManager Instance;

    [SerializeField] private GameObject exitConfirmPopup;

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