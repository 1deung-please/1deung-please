using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchButtonManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private EndingManager endingManager;
    [SerializeField] private GameObject scratchButton;

    private void Start()
    {
        scratchButton.SetActive(false);
    }

    public void ShowButton()
    {
        scratchButton.SetActive(true);
    }

    public void OnScratchButtonClick()
    {
        scratchButton.SetActive(false);
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        for (float a = 0; a <= 1; a += Time.deltaTime)
        {
            fadeImage.color = new Color(1, 1, 1, a);
            yield return null;
        }

        endingManager.DetermineEnding();
    }
}