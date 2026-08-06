using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchButtonManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private EndingManager endingManager;

    public void OnScratchButtonClick()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        for(float a = 0; a <= 1; a += Time.deltaTime)
        {
            fadeImage.color = new Color(0,0,0,a);
            yield return null;
        }

        endingManager.DetermineEnding();
    }
}