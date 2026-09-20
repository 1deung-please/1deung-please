using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommonEndingManager : MonoBehaviour
{
    [Header("Dialogue UI (조상신 대화)")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Ending UI")]
    public TMP_Text titleText;
    public TMP_Text meritPointText;
    public TMP_Text ptText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    [Header("Signboard Animation")]
    public RectTransform signboard;
    public float moveDuration = 3f;
    public float startOffsetY = 1000f;

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;

    [Header("Merit Point Animation")]
    public float numberDelay = 0.3f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip signboardSound;
    public AudioClip numberSound;

    private Vector2 targetPosition;

    private string[] ancestorDialogues = new string[]
    {
        "그래... 처음 보는구나.",
        "내가 바로 네 조상이다.",
        "내가 널 참 오랫동안 지켜보고 있었지... 갓난아기일 때부터 회사에 치이는 지금까지...",
        "얼마나 고생이 많았느냐. 난 널 도와주러 온 사람이야.",
        "그럼 어디, 지난 시간동안 얼마나 공덕을 쌓아왔는지 볼까."
    };
    private int dialogueIndex = 0;
    private bool isDialogueEnding = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence = "";

    // 점수판 연출(ShowEndingInfo)이 진행 중인지 여부
    private bool isScoreInfoPlaying = false;
    // 연출이 전부 끝나서 이제 클릭하면 엔딩으로 넘어갈 수 있는 상태인지
    private bool scoreInfoFinished = false;
    private Coroutine scoreInfoCoroutine;

    void Start()
    {
        if (titleText != null)
            titleText.gameObject.SetActive(false);

        if (meritPointText != null)
            meritPointText.gameObject.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
        
        if (ptText != null)
            ptText.gameObject.SetActive(false);

        if (signboard != null)
        {
            targetPosition = signboard.anchoredPosition;
            signboard.anchoredPosition = new Vector2(
                targetPosition.x,
                targetPosition.y + startOffsetY
            );
        }
        else
        {
            Debug.LogError("Signboard가 연결되지 않았습니다.");
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            ShowNextDialogue();
        }
    }

    void Update()
    {
        if (!isDialogueEnding && dialoguePanel != null && dialoguePanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                OnDialogueClick();
            }
        }
    }

    private void OnDialogueClick()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        dialogueIndex++;

        if (dialogueIndex < ancestorDialogues.Length)
        {
            ShowNextDialogue();
        }
        else
        {
            isDialogueEnding = true;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);

            if (signboard != null)
            {
                StartCoroutine(MoveSignboard());
            }
        }
    }

    private void ShowNextDialogue()
    {
        if (dialogueText != null)
        {
            currentSentence = ancestorDialogues[dialogueIndex];

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(currentSentence));
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private IEnumerator MoveSignboard()
    {
        Debug.Log("Signboard 이동 시작");

        if (audioSource != null && signboardSound != null)
        {
            audioSource.PlayOneShot(signboardSound);
        }

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

            signboard.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        signboard.anchoredPosition = targetPosition;

        Debug.Log("Signboard 도착");

        // 표지판이 도착한 후 텍스트 연출 시작
        isScoreInfoPlaying = true;
        scoreInfoCoroutine = StartCoroutine(ShowEndingInfo());
    }

    private IEnumerator ShowEndingInfo()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            yield break;
        }

        GameData gameData = GameManager.Instance.gameData;

        int totalPlayCount = gameData.playCycle;

        if (titleText != null)
        {
            titleText.text =
                totalPlayCount + "번째 플레이...모은 공덕 포인트";

            titleText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (meritPointText != null)
        {
            meritPointText.gameObject.SetActive(true);

            int totalPoint = gameData.meritPoint;

            string pointString = totalPoint.ToString();

            meritPointText.text = "";

            for (int i = pointString.Length - 1; i >= 0; i--)
            {
                if (ptText != null && !ptText.gameObject.activeSelf)
                {
                    ptText.gameObject.SetActive(true);
                }

                string revealedNumber = pointString.Substring(i, pointString.Length - i);

                meritPointText.text = revealedNumber;

                if (audioSource != null && numberSound != null)
                {
                    audioSource.PlayOneShot(numberSound);
                }

                yield return new WaitForSeconds(numberDelay);
            }

            if (ptText != null)
            {
                ptText.gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.3f);

        ShowFullPointText(gameData);

        // 연출 자연 종료: 이제부터 클릭하면 엔딩으로 넘어감
        isScoreInfoPlaying = false;
        scoreInfoFinished = true;
    }

    // 연출을 건너뛰고 모든 텍스트를 즉시 최종 상태로 보여줌
    private void SkipScoreInfo()
    {
        if (scoreInfoCoroutine != null)
            StopCoroutine(scoreInfoCoroutine);

        GameData gameData = GameManager.Instance.gameData;

        if (titleText != null)
        {
            titleText.text = gameData.playCycle + "번째... 플레이 모은 공덕 포인트";
            titleText.gameObject.SetActive(true);
        }

        if (meritPointText != null)
        {
            meritPointText.text = gameData.meritPoint.ToString();
            meritPointText.gameObject.SetActive(true);
        }

        if (ptText != null)
            ptText.gameObject.SetActive(true);

        ShowFullPointText(gameData);

        isScoreInfoPlaying = false;
        scoreInfoFinished = true;
    }    

    private void ShowFullPointText(GameData gameData)
    {
        if (nameText != null)
        {
            nameText.text =
                "이걸 안 비켜?\n" +
                "출격! 논리요새\n" +
                "주워줘, 쓰레기!";

            nameText.alignment = TextAlignmentOptions.Left;
            nameText.gameObject.SetActive(true);
        }

        if (scoreText != null)
        {
            scoreText.text =
                gameData.miniGame2Score + "\n" +
                gameData.miniGame3Score + "\n" +
                gameData.miniGame1Score;

            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.gameObject.SetActive(true);
        }
    }

    // Signboard 버튼 OnClick에 연결: 연출 중이면 스킵, 연출이 끝난 상태면 엔딩으로 진행
    public void OnScoreBoardClick()
    {
        if (isScoreInfoPlaying)
        {
            SkipScoreInfo();
            return;
        }

        if (!scoreInfoFinished)
        {
            // 아직 표지판 연출조차 끝나지 않은 상태 - 클릭 무시
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.DetermineEnding();
    }
}