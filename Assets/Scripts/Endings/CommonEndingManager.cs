using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommonEndingManager : MonoBehaviour
{
    [Header("Ending UI")]
    public TMP_Text titleText;
    public TMP_Text meritPointText;
    public TMP_Text pointText;

    [Header("Signboard Animation")]
    public RectTransform signboard;
    public float moveDuration = 3f;
    public float startOffsetY = 1000f;

    [Header("Merit Point Animation")]
    public float numberDelay = 0.3f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip signboardSound;
    public AudioClip numberSound;

    private Vector2 targetPosition;

    void Start()
    {
        if (titleText != null)
            titleText.gameObject.SetActive(false);

        if (meritPointText != null)
            meritPointText.gameObject.SetActive(false);

        if (pointText != null)
            pointText.gameObject.SetActive(false);

        if (signboard != null)
        {
            Debug.Log("Signboard 연결됨");
            targetPosition = signboard.anchoredPosition;

            // 원래 위치보다 위에서 시작
            signboard.anchoredPosition = new Vector2(
                targetPosition.x,
                targetPosition.y + startOffsetY
            );

            StartCoroutine(MoveSignboard());
        }
        else
        {
            Debug.LogError("Signboard가 연결되지 않았습니다.");
        }
    }

    private IEnumerator MoveSignboard()
    {
        Debug.Log("Signboard 이동 시작");

        float elapsed = 0f;
        Vector2 startPosition = signboard.anchoredPosition;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / moveDuration
            );

            // Ease Out
            t = 1f - Mathf.Pow(1f - t, 3f);

            signboard.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            yield return null;
        }

        signboard.anchoredPosition = targetPosition;

        Debug.Log("Signboard 도착");

        // 표지판이 도착한 후 텍스트 연출 시작
        StartCoroutine(ShowEndingInfo());
    }

    private IEnumerator ShowEndingInfo()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            yield break;
        }

        GameData gameData = GameManager.Instance.gameData;

       int totalPlayCount = 0;

        foreach (int count in gameData.playCount)
        {
            totalPlayCount += count;
        }

        if (titleText != null)
        {
            titleText.text =
                totalPlayCount + "번째.. 쌓은 공덕 포인트";

            titleText.gameObject.SetActive(true);
        }

        if (meritPointText != null)
        {
            meritPointText.gameObject.SetActive(true);

            int totalPoint = gameData.meritPoint;

            string pointString = totalPoint.ToString();

            // 처음에는 아무것도 표시하지 않음
            meritPointText.text = "";

            // 일의 자리 → 십의 자리 → 백의 자리 → ...
            for (int i = pointString.Length - 1; i >= 0; i--)
            {
                string revealedNumber =
                    pointString.Substring(
                        i,
                        pointString.Length - i
                    );

                meritPointText.text =
                    revealedNumber + " pt";

                // 숫자 하나 공개될 때 효과음
                if (audioSource != null &&
                    numberSound != null)
                {
                    audioSource.PlayOneShot(numberSound);
                }

                yield return new WaitForSeconds(
                    numberDelay
                );
            }
        }

        if (pointText != null)
        {
            pointText.text =
                "이걸 안 비켜? " +
                gameData.miniGame2Score + " PT\n" +

                "출격! 논리요새 " +
                gameData.miniGame3Score + " PT\n" +

                "주워줘, 쓰레기! " +
                gameData.miniGame1Score + " PT";

            // 한 번에 공개
            pointText.gameObject.SetActive(true);
        }
    }

    public void OnScoreBoardClick()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        EndingManager endingManager =
            FindFirstObjectByType<EndingManager>();

        if (endingManager == null)
        {
            Debug.LogError(
                "EndingManager를 찾을 수 없습니다."
            );
            return;
        }

        endingManager.DetermineEnding();
    }
}