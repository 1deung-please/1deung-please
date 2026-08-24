using TMPro;
using UnityEngine;
using System.Collections;

public class TimerUI : MonoBehaviour
{
    public TMP_Text timerText;

    [Header("Timer Animation")]
    public float startY = 1000f;
    public float moveDuration = 1f;

    private RectTransform rectTransform;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip timerMoveSound;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        StartCoroutine(MoveTimer());
    }

    private IEnumerator MoveTimer()
    {
        Vector2 endPosition = rectTransform.anchoredPosition;

        Vector2 startPosition = endPosition;
        startPosition.y = startY;

        rectTransform.anchoredPosition = startPosition;

        if (audioSource != null && timerMoveSound != null)
        {
            audioSource.PlayOneShot(timerMoveSound);
        }

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        rectTransform.anchoredPosition = endPosition;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float time = GameManager.Instance.gameData.globalTimeRemaining;

        int minute = Mathf.FloorToInt(time / 60);
        int second = Mathf.FloorToInt(time % 60);

        timerText.text = $"{minute:00}:{second:00}";

        if (time <= 30)
        {
            float alpha = Mathf.PingPong(Time.time * 3f, 1f);

            Color c = Color.red;
            c.a = alpha;

            timerText.color = c;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}