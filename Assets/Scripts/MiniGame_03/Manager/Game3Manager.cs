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
    public TMP_Text attackButtonText;

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

    //터치하여 시작하기 깜빡임
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

    //단어 버튼 글자 선택 저장
    public void SelectChar(char c)
    {
        if (!gameStarted || gameEnded)
            return;

        selectedChars.Add(c);
    }

    //공격 버튼
    public void Attack()
    {
        if (!gameStarted || gameEnded)
            return;

        if (attackButtonText != null)
            attackButtonText.gameObject.SetActive(false);

        string playerAnswer = new string(selectedChars.ToArray());
        string correctAnswer = ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        if (playerAnswer == correctAnswer)
        {
            Debug.Log("정답");

            enemy.Damage(20);

           if (!gameEnded)
           {
                ProblemManager.Instance.NextProblem();
           }
        }
        else
        {
            Debug.Log("오답");
        }

        selectedChars.Clear();
    }

    //성공 처리
    public void GameSuccess()
    {
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

    //실패 처리
    public void GameFail()
    {
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

    //결과창
    private void ShowResult(bool isSuccess, int earnedPoint)
    {
        if (gameEnded)
            return;

        gameEnded = true;

        //전역 타이머 일시정지
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseTimer();
        }


        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (attackButtonText != null)
            attackButtonText.gameObject.SetActive(false);

        if (resultTitleText != null)
            resultTitleText.text = isSuccess ?  "<color=#7CFF5A>SUCCESS!</color>" : "<color=#FF4D4D>FAIL</color>";

        if (pointText != null)
            pointText.text = earnedPoint.ToString();;

        Time.timeScale = 0f;

        //전역 타이머가 미니게임 도중 끝났을 경우 미니게임이 끝났을 때
        if (GameManager.Instance != null && GameManager.Instance.IsPendingEndingTransition())
        {
            StartCoroutine(AutoReturnToLobbyAfterDelay());
        }
    }

    //결과창 표시 될 때 단어 버튼 사라짐
    private void ClearWordButtons()
    {
        if (AnswerManager.Instance != null)
            AnswerManager.Instance.Clear();

        if (wordPanel != null)
            wordPanel.gameObject.SetActive(false);
    }

    //다시 하기 버튼 눌렀을 때
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

    //거리로 돌아가기 버튼 눌렀을 때
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

    //전역 타이머가 미니게임 도중 끝났을 경우 미니게임이 끝났을 때
    IEnumerator AutoReturnToLobbyAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        GameManager.Instance.ReturnToLobby();
        //결과창 2초 표시 후 자동 로비로 이동
    }

    //게임 시작할 때
    private void StartGame()
    {
        StartCoroutine(FlashThenStart());
    }

    //터치하여 시작하기 버튼 눌렀을 때
    IEnumerator FlashThenStart()
    {
        //터치하여 시작하기 텍스트 깜빡임 멈추기
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        //화면 Flash 효과
        if (flashPanel != null)
        {
            Image flashImage = flashPanel.GetComponent<Image>();

            flashPanel.SetActive(true);
            flashImage.color = new Color(1, 1, 1, 1);

            float t = 0f;

            while (t < 0.3f)
            {
                t += Time.unscaledDeltaTime;

                flashImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t / 0.3f));

                yield return null;
            }

            flashPanel.SetActive(false);
        }

        //Flash 끝난 후 게임 시작
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMiniGameStart();
            GameManager.Instance.RecordMiniGamePlay(3);
        }

        gameStarted = true;

        if (ReadyPanel != null)
            ReadyPanel.SetActive(false);

        if (AnswerManager.Instance != null && AnswerManager.Instance.heroThinkText != null)
        {
            AnswerManager.Instance.heroThinkText.text = "";
        }

        ProblemManager.Instance.NextProblem();

        Time.timeScale = 1f;
    }
}