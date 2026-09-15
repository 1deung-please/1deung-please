using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchButtonManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject scratchButton;
    [SerializeField] private GameObject scratchPanel;

    private bool isClicked = false;

    private void Start()
    {
        if (scratchButton != null)
            scratchButton.SetActive(false);

        if (fadeImage != null)
            fadeImage.color = new Color(1, 1, 1, 0);
    }

    public void ShowButton()
    {
        // 이미 클릭했다면 절대 다시 띄우지 않음
        if (isClicked)
            return;

        if (scratchButton != null)
            scratchButton.SetActive(true);
    }

    public void OnScratchButtonClick()
    {
        // 중복 클릭 방지
        if (isClicked)
            return;

        isClicked = true;

        if (scratchButton != null)
            scratchButton.SetActive(false);

        if (scratchPanel != null)
            scratchPanel.SetActive(true);
    }

    public void StartEndingFade()
    {
        Debug.Log("ScratchButtonManager → StartEndingFade 실행");

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        Debug.Log("FadeRoutine 시작");
        float duration = 1.0f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Clamp01(timer / duration);

            if (fadeImage != null)
                fadeImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(1, 1, 1, 1);
            fadeImage.raycastTarget = false;
        }

        Debug.Log("FadeRoutine 완료");

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("Ending_Common");
        }
        else
        {
            Debug.LogError("SceneLoader.Instance가 없습니다.");
        }
    }
}