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

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
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
            fadeImage.color = new Color(1, 1, 1, 1);

        Debug.Log("흰색 페이드 완료 → 엔딩 결정");

        if (endingManager != null)
        {
            endingManager.DetermineEnding();
        }
        else
        {
            Debug.LogError("EndingManager가 연결되지 않았습니다.");
        }
    }

    public void StartEndingFade()
    {
        StartCoroutine(FadeRoutine());
    }
}