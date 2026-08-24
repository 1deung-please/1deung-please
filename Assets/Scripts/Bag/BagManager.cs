using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BagManager : MonoBehaviour
{
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject restartConfirmPanel;
    [SerializeField] private GameObject recordBookPanel; // 엔딩+업적 통합 도감 패널 
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
        recordBookPanel.SetActive(false);
        restartConfirmPanel.SetActive(false);
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
        recordBookPanel.SetActive(true); // 가방 열면 바로 도감(엔딩+업적) 표시
        restartConfirmPanel.SetActive(false);
        Time.timeScale = 0f;
    }
    public void CloseBag()
    {
        PlaySfx(closeSfx);
        bagPanel.SetActive(false);
        recordBookPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OpenRestartConfirm()
    {
        PlaySfx(buttonSfx);
        recordBookPanel.SetActive(false);
        restartConfirmPanel.SetActive(true);
    }
    public void CancelRestart()
    {
        PlaySfx(buttonSfx);
        restartConfirmPanel.SetActive(false);
        recordBookPanel.SetActive(true);
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
}