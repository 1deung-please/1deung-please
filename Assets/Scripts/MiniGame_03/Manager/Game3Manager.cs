using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game3Manager : MonoBehaviour
{
    public static Game3Manager Instance;
    
    [Header("기존 게임 연결")]
    public EnemyManager enemy;

    [Header("게임 UI")]
    public Transform wordPanel;
    public Transform answerPanel;

    [Header("시작 화면")]
    public GameObject ReadyPanel;
    public Button readyButton;
    public TMP_Text touchToStart;

    private Coroutine blinkCoroutine;

    private bool gameStarted = false;
    
    [Header("결과 화면")]
    public GameObject resultPanel;
    public TMP_Text resultTitleText;
    public TMP_Text pointText;
    public Button retryButton;
    public Button returnButton;

    [Header("거리 씬 이름")]
    public string streetSceneName = "Lobby";

    [Header("화면 Flash")]
    public GameObject flashPanel;

    [Header("결과 버튼 효과음")]
    public AudioSource audioSource;
    public AudioClip buttonSfx;

    private List<char> selectedChars = new List<char>();
    private bool gameEnded = false;
    public bool IsGameEnded => gameEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 0f;
        gameStarted = false;

        if (ReadyPanel != null)
            ReadyPanel.SetActive(true);
        
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (AnswerManager.Instance != null)
            AnswerManager.Instance.Clear();

        if (readyButton != null)
            readyButton.onClick.AddListener(StartGame);

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryGame);

            Image retryImage = retryButton.GetComponent<Image>();

            if (retryImage != null)
                retryImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToStreet);

            Image returnImage = returnButton.GetComponent<Image>();

            if (returnImage != null)
                returnImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (touchToStart != null)
            blinkCoroutine = StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.unscaledTime * 1.5f, 1f);
            Color c = touchToStart.color;
            c.a = alpha;
            touchToStart.color = c;
            yield return null;
        }
    }

    // 글자 선택
    public void SelectChar(char c)
    {
        if (!gameStarted || gameEnded)
            return;

        selectedChars.Add(c);

        Debug.Log(new string(selectedChars.ToArray()));
    }

    // 공격 버튼
    public void Attack()
    {
        if (!gameStarted || gameEnded)
            return;

        string playerAnswer = new string(selectedChars.ToArray());

        string correctAnswer =
            ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        if (playerAnswer == correctAnswer)
        {
            Debug.Log("정답");

            enemy.Damage(20);

           if (!gameEnded)
            ProblemManager.Instance.NextProblem();
        }
        else
        {
            Debug.Log("오답");
        }

        selectedChars.Clear();
    }

    // 성공 처리
    public void GameSuccess()
    {
        // 로비에서 정상적으로 들어온 경우에만 저장 및 업적 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteMiniGame3(true);

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnMiniGameResult(MiniGameKind.LogicFortress,true);
            }
        }
        else
        {
            Debug.Log("미니게임 재시작 상태: 결과 저장과 업적 처리를 생략합니다.");
        }

        ShowResult(true, 700);
    }


    // 실패 처리
    public void GameFail()
    {
        // 로비에서 정상적으로 들어온 경우에만 저장 및 업적 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteMiniGame3(false);

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnMiniGameResult(MiniGameKind.LogicFortress,false);
            }
        }
        else
        {
            Debug.Log("미니게임 재시작 상태: 결과 저장과 업적 처리를 생략합니다.");
        }

        ShowResult(false, 0);
    }

    private void ShowResult(bool isSuccess, int earnedPoint)
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
        }

        ClearWordButtons();

        if (ProblemManager.Instance != null &&
        ProblemManager.Instance.DomitgirlText != null)
        {
            ProblemManager.Instance.DomitgirlText.SetActive(false);
        }

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultTitleText != null)
            resultTitleText.text = isSuccess ? "성공!" : "실패!";

        if (pointText != null)
            pointText.text = "획득 공덕포인트\n" + earnedPoint;

        Time.timeScale = 0f;

        if (GameManager.Instance != null &&
        GameManager.Instance.IsPendingEndingTransition())
    {
        StartCoroutine(AutoReturnToLobbyAfterDelay());
    }
    }

    private void ClearWordButtons()
{
    if (AnswerManager.Instance != null)
        AnswerManager.Instance.Clear();

    if (wordPanel)
    {
        for (int i = wordPanel.childCount - 1; i >= 0; i--)
        {
            Transform child = wordPanel.GetChild(i);

            if (child != null && child.GetComponent<WordButton>() != null)
                Destroy(child.gameObject);
        }
    }

    if (answerPanel)
    {
        for (int i = answerPanel.childCount - 1; i >= 0; i--)
        {
            Transform child = answerPanel.GetChild(i);

            if (child != null && child.GetComponent<WordButton>() != null)
                Destroy(child.gameObject);
        }
    }
}

    private void RetryGame()
    {
        StartCoroutine(RetryGameRoutine());
    }

    private IEnumerator RetryGameRoutine()
    {
        if (audioSource != null && buttonSfx != null)
            audioSource.PlayOneShot(buttonSfx);

        yield return new WaitForSecondsRealtime(0.2f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToStreet()
    {
         StartCoroutine(ReturnToStreetRoutine());
    }

    private IEnumerator ReturnToStreetRoutine()
{
    if (audioSource != null && buttonSfx != null)
        audioSource.PlayOneShot(buttonSfx);

    yield return new WaitForSecondsRealtime(0.2f);

    Time.timeScale = 1f;

    if (GameManager.Instance != null)
    {
        GameManager.Instance.ReturnToLobby();
        yield break;
    }

    SceneManager.LoadScene(streetSceneName);
}

    IEnumerator AutoReturnToLobbyAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        GameManager.Instance.ReturnToLobby();
    }

    private void StartGame()
    {
        StartCoroutine(FlashThenStart());
    }

IEnumerator FlashThenStart()
{
    // 터치하여 시작하기 Blink 중지
    if (blinkCoroutine != null)
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }

    // 화면 Flash 효과
    if (flashPanel != null)
    {
        Image flashImage = flashPanel.GetComponent<Image>();

        flashPanel.SetActive(true);
        flashImage.color = new Color(1, 1, 1, 1);

        float t = 0f;

        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;

            flashImage.color = new Color(
                1,
                1,
                1,
                Mathf.Lerp(1, 0, t / 0.3f)
            );

            yield return null;
        }

        flashPanel.SetActive(false);
    }

    // Flash 끝난 후 게임 시작
    if (GameManager.Instance != null)
    {
        GameManager.Instance.OnMiniGameStart();
        GameManager.Instance.RecordMiniGamePlay(3);
    }

    gameStarted = true;

    if (ReadyPanel != null)
        ReadyPanel.SetActive(false);

    if (AnswerManager.Instance != null &&
        AnswerManager.Instance.heroThinkText != null)
    {
        AnswerManager.Instance.heroThinkText.text = "";
    }

    ProblemManager.Instance.NextProblem();

    Time.timeScale = 1f;
}
}