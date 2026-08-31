using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("로딩 화면 UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image progressBarFill; // Image Type: Filled
    [SerializeField] private TMP_Text loadingMessageText;
    [SerializeField] private RectTransform npcCharacter; // 게이지 따라 움직일 NPC
    [SerializeField] private RectTransform progressBarRect; // 게이지 바 전체 RectTransform (NPC 이동 범위 계산용)
    [SerializeField] private float npcStartX = -501f; // NPC 시작 X 좌표
    [SerializeField] private float npcEndX = 523f;     // NPC 도착 X 좌표

    [Header("멘트 (5초마다 전환, 반복)")]
    [SerializeField]
    private string[] loadingMessages = new string[]
    {
        "로딩 중...",
        "로또 사러 가는 중...",
        "로또 긁는 중..."
    };
    [SerializeField] private float messageInterval = 5f;

    [Header("가짜 로딩 시간 (연출용 최소 시간, 초)")]
    [SerializeField] private float minLoadingDuration = 3f;

    private bool isLoading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    // 기존 방식: 로딩 화면 없이 즉시 전환
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;
        SceneManager.LoadScene(sceneName);
        StartCoroutine(ResetLoadingFlagNextFrame());
    }

    // 로딩 화면과 함께 전환 (엔딩 진입 등에 사용)
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    IEnumerator ResetLoadingFlagNextFrame()
    {
        yield return null;
        isLoading = false;
        Time.timeScale = 1f;
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        yield return null; // 레이아웃이 갱신될 때까지 한 프레임 대기
        SetProgress(0f); // 게이지와 NPC 위치를 시작점으로 초기화

        Coroutine messageCoroutine = StartCoroutine(CycleMessages());

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        // 실제 로딩 진행률(0~0.9)과 최소 연출 시간을 함께 고려해서 게이지 채우기
        while (elapsed < minLoadingDuration || op.progress < 0.9f)
        {
            elapsed += Time.deltaTime;

            float realProgress = Mathf.Clamp01(op.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsed / minLoadingDuration);
            float displayProgress = Mathf.Min(realProgress, timeProgress);

            SetProgress(displayProgress);

            yield return null;
        }

        SetProgress(1f);
        yield return new WaitForSeconds(0.2f); // 100% 상태 잠깐 보여주기

        StopCoroutine(messageCoroutine);

        op.allowSceneActivation = true;

        // 씬 전환 완료까지 대기
        while (!op.isDone)
            yield return null;

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        isLoading = false;
        Time.timeScale = 1f;
    }

    void SetProgress(float value)
    {
        if (progressBarFill != null)
            progressBarFill.fillAmount = value;

        // 게이지 진행률에 맞춰 NPC 위치 이동 (Inspector에서 지정한 시작/끝 X좌표 기준)
        if (npcCharacter != null)
        {
            Vector2 pos = npcCharacter.anchoredPosition;
            pos.x = Mathf.Lerp(npcStartX, npcEndX, value);
            npcCharacter.anchoredPosition = pos;
        }
    }

    IEnumerator CycleMessages()
    {
        if (loadingMessages == null || loadingMessages.Length == 0) yield break;

        int index = 0;
        while (true)
        {
            if (loadingMessageText != null)
                loadingMessageText.text = loadingMessages[index];

            index = (index + 1) % loadingMessages.Length;
            yield return new WaitForSeconds(messageInterval);
        }
    }
}