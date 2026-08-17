using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScratchButtonManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private EndingManager endingManager;
    [SerializeField] private GameObject scratchButton;
    [SerializeField] private GameObject scratchPanel;

    private bool scratchStarted = false;

    private void Start()
    {
        scratchButton.SetActive(false);

        if (scratchPanel != null)
            scratchPanel.SetActive(false);

        if (fadeImage != null)
            fadeImage.color = new Color(1, 1, 1, 0);
    }

    public void ShowButton()
    {
        if (scratchStarted)
            return;

        scratchButton.SetActive(true);
    }

    public void OnScratchButtonClick()
    {
        if (scratchStarted)
            return;

        scratchStarted = true;

        scratchButton.SetActive(false);

        if (scratchPanel != null)
            scratchPanel.SetActive(true);
    }

    public void StartEndingFade()
    {
        StartCoroutine(FadeToEnding());
    }

    IEnumerator FadeToEnding()
    {
        // 흰색으로 점점 밝아짐
        for (float a = 0; a <= 1; a += Time.deltaTime)
        {
            fadeImage.color = new Color(1, 1, 1, a);
            yield return null;
        }

        fadeImage.color = new Color(1, 1, 1, 1);

        SceneLoader.Instance.LoadScene("Ending_Common");
    }
}