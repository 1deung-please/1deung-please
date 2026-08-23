using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    [SerializeField] private GameObject bagPanel;               //가방 전체
    [SerializeField] private GameObject menuPanel;              //가방 메뉴
    [SerializeField] private GameObject restartConfirmPanel;    //다시 시작 확인창
    [SerializeField] private GameObject endingBookPanel;        //엔딩 도감
    [SerializeField] private GameObject achievementBookPanel;   //업적 도감
    [SerializeField] private GameObject whiteOverlay;

    [Header("가방 버튼")]
    [SerializeField] private Button bagButton;          //가방 열기 버튼
    [SerializeField] private Button closeButton;        //가방 닫기 버튼
    [SerializeField] private Button restartButton;      //다시 시작 버튼
    [SerializeField] private Button xButton;            //다시 시작 확인창 닫기 버튼

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
        whiteOverlay.SetActive(false);
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

    //가방 열기
    public void OpenBag()
    {
        PlaySfx(openSfx); 

        bagPanel.SetActive(true);
        menuPanel.SetActive(true);

        whiteOverlay.SetActive(false);
        restartConfirmPanel.SetActive(false);
        endingBookPanel.SetActive(false);
        achievementBookPanel.SetActive(false);
    }
    //가방 닫기
    public void CloseBag()
    {
        PlaySfx(closeSfx); 

        bagPanel.SetActive(false);
    }

    //다시 시작 확인창 열기
    public void OpenRestartConfirm()
    {
        PlaySfx(buttonSfx);

        menuPanel.SetActive(false);
        whiteOverlay.SetActive(true);
        restartConfirmPanel.SetActive(true);
    }
    //다시 시작 취소
    public void CancelRestart()
    {
        PlaySfx(buttonSfx);

        restartConfirmPanel.SetActive(false);
        whiteOverlay.SetActive(false);
        menuPanel.SetActive(true);
    }
    //다시 시작 확인
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


    //엔딩 도감 열기
    public void OpenEndingBook()
    {
        PlaySfx(buttonSfx);

        menuPanel.SetActive(false);
        endingBookPanel.SetActive(true);
    }
    //엔딩 도감 닫기
    public void CloseEndingBook()
    {
        PlaySfx(buttonSfx);

        endingBookPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    //업적 도감 열기
    public void OpenAchievementBook()
    {
        PlaySfx(buttonSfx);

        menuPanel.SetActive(false);
        achievementBookPanel.SetActive(true);
    }
    //업적 도감 닫기
    public void CloseAchievementBook()
    {
        PlaySfx(buttonSfx);
    
        achievementBookPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
