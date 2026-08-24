using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;

    [Header("텍스트")]
    public TMP_Text heroThinkText;
    public TMP_Text answerText;

    [Header("힌트")]
    public GameObject hintText;

    [Header("버튼 이미지")]
    public Image cancelButtonImage;
    public Image attackButtonImage;

    [Header("주인공 말풍선")]
    public GameObject heroThinkBubble;  // 평소 생각 말풍선 전체
    public GameObject heroText;
    public TMP_Text heroTextText;

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip correctSfx;
    public AudioClip wrongSfx;
    public AudioClip wordButtonSfx;
    public AudioClip attackButtonSfx;

    private List<WordButton> selectedButtons = new List<WordButton>();
    private List<char> selectedLetters = new List<char>();

    private bool isChecking = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (heroText != null)
            heroText.SetActive(false);

        if (heroThinkBubble != null)
            heroThinkBubble.SetActive(true);

        // Cancel / Attack 투명 영역 클릭 방지
        if (cancelButtonImage != null)
            cancelButtonImage.alphaHitTestMinimumThreshold = 0.1f;

        if (attackButtonImage != null)
            attackButtonImage.alphaHitTestMinimumThreshold = 0.1f;

        if (heroThinkText != null)
            heroThinkText.text = "";

        if (answerText != null)
            answerText.text = "";

        if (hintText != null)
            hintText.SetActive(false);
    }

    // 단어 버튼 클릭
    public void SelectWord(WordButton button, char letter)
    {
        if (isChecking)
            return;

        selectedButtons.Add(button);
        selectedLetters.Add(letter);
        
        if (audioSource != null && wordButtonSfx != null)
            audioSource.PlayOneShot(wordButtonSfx);

        // 선택한 글자를 HeroThinkText에 표시
        if (heroThinkText != null)
        {
            heroThinkText.text =
                new string(selectedLetters.ToArray());
        }
    }

    public string GetAnswer()
    {
        return new string(selectedLetters.ToArray());
    }

    // 취소 버튼
    public void Clear()
    {
        if (isChecking)
            return;

        foreach (WordButton button in selectedButtons)
        {
            if (button != null)
                button.Restore();
        }

        selectedButtons.Clear();
        selectedLetters.Clear();

        if (heroThinkText != null)
            heroThinkText.text = "";
    }

    // Attack 버튼
    public void CheckAnswer()
    {
        Debug.Log("Attack 클릭됨");
        
        if (isChecking)
            return;

        if (ProblemManager.Instance == null ||
            ProblemManager.Instance.currentProblem == null)
            return;

        if (audioSource != null && attackButtonSfx != null)
            audioSource.PlayOneShot(attackButtonSfx);
        
        StartCoroutine(CheckAnswerRoutine());
    }

    private IEnumerator CheckAnswerRoutine()
    {
        isChecking = true;

        string playerAnswer = GetAnswer();

        string correctAnswer =
            ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        // =====================
        // 정답
        // =====================
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("정답!");

            if (audioSource != null && correctSfx != null)
                audioSource.PlayOneShot(correctSfx);

            selectedButtons.Clear();
            selectedLetters.Clear();

            if (heroThinkText != null)
                heroThinkText.text = "";

            // 평소 생각 말풍선 숨기기
            if (heroThinkBubble != null)
                heroThinkBubble.SetActive(false);

            // 정답 말풍선 표시
            if (heroText != null)
                heroText.SetActive(true);

            if (heroTextText != null)
                heroTextText.text = ProblemManager.Instance.currentProblem.answer;

            if (Game3Manager.Instance != null &&
                Game3Manager.Instance.enemy != null)
            {
                Game3Manager.Instance.enemy.Damage(20);
            }

            // 게임이 끝났으면
            if (Game3Manager.Instance != null &&
            Game3Manager.Instance.IsGameEnded)
            {
                if (heroText != null)
                    heroText.SetActive(false);

                isChecking = false;
                yield break;
            }

            // 1초 동안 정답 말풍선
            yield return new WaitForSeconds(1f);

            // 정답 말풍선 숨김
            if (heroText != null)
                heroText.SetActive(false);

            // 평소 생각 말풍선 다시 표시
            if (heroThinkBubble != null)
                heroThinkBubble.SetActive(true);

            ProblemManager.Instance.NextProblem();
        }

        // =====================
        // 오답
        // =====================
        else
        {
            Debug.Log("오답!");

            if (audioSource != null && wrongSfx != null)
                audioSource.PlayOneShot(wrongSfx);

            // 힌트 이미지 켜기
            if (hintText != null)
                hintText.SetActive(true);

            // 실제 정답 글자 표시
            if (answerText != null)
            {
                answerText.gameObject.SetActive(true);
                answerText.text = ProblemManager.Instance.currentProblem.answer;
            }

            yield return new WaitForSeconds(3f);

            // 3초 후 힌트와 정답 숨기기
            if (answerText != null)
            {
                answerText.text = "";
                answerText.gameObject.SetActive(false);
            }

            if (hintText != null)
                hintText.SetActive(false);

            selectedButtons.Clear();
            selectedLetters.Clear();

            if (heroThinkText != null)
                heroThinkText.text = "";

            if (Game3Manager.Instance != null &&
            !Game3Manager.Instance.IsGameEnded)
            {
                ProblemManager.Instance.NextProblem();
            }
        }
            isChecking = false;
    }
}