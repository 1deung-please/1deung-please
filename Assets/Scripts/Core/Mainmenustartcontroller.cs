using System.Collections;
using UnityEngine;
using TMPro;

public class MainMenuStartController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text touchToStartText; // "복권 긁으러 가기"

    [Header("이동할 씬 이름")]
    public string tutorialSceneName = "Tutorial";

    [Header("깜빡임 속도 (낮을수록 천천히)")]
    public float blinkSpeed = 1.2f;

    private Coroutine blinkCoroutine;
    private bool isTransitioning = false;

    void Start()
    {
        if (touchToStartText != null)
            blinkCoroutine = StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetMouseButtonDown(0))
        {
            isTransitioning = true;
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            SceneLoader.Instance.LoadScene(tutorialSceneName);
        }
    }

    // MiniGame01Controller의 BlinkText와 동일한 방식
    IEnumerator BlinkText()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color c = touchToStartText.color;
            c.a = alpha;
            touchToStartText.color = c;
            yield return null;
        }
    }
}