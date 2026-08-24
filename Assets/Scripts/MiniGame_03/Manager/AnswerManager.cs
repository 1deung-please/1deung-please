using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;

    [Header("힌트")]
    public GameObject hintText;     //오답일 때 나타나는 정답 화면

    [Header("버튼 이미지")]
    public Image cancelButtonImage;     //취소 버튼 이미지
    public Image attackButtonImage;     //말하기 버튼 이미지

    [Header("주인공 말풍선")]
    public GameObject heroThinkBubble;  //주인공 생각 말풍선
    public GameObject heroBubble;         //정답일 때 표시되는 주인공 기본 말풍선
    public TMP_Text heroTextText;       //정답일 때 표시되는 주인공 기본 말풍선 텍스트
    public TMP_Text heroThinkText;      //주인공 생각 말풍선 텍스트
    public TMP_Text answerText;         //오답일 때 보여주는 정답 텍스트

    [Header("효과음")]
    public AudioSource audioSource; 
    public AudioClip correctSfx;        //정답 효과음
    public AudioClip wrongSfx;          //오답 효과음
    public AudioClip wordButtonSfx;     //단어 버튼 클릭 효과음
    public AudioClip attackButtonSfx;   //말하기 버튼 클릭 효과음

    //선택한 글자들을 순서대로 저장
    private List<WordButton> selectedButtons = new List<WordButton>();
    private List<char> selectedLetters = new List<char>();

    [Header("도믿걸 오답 애니메이션")]
    public GameObject domitgirl;
    public GameObject domitgirlAngry;

    private bool isChecking = false;

    private void Awake()
    {
        Instance = this;
    }

    //게임 시작할 때
    private void Start()
    {
        if (heroBubble != null)
            heroBubble.SetActive(false);

        if (heroThinkBubble != null)
            heroThinkBubble.SetActive(true);

        if (hintText != null)
            hintText.SetActive(false);

        //버튼 이미지 투명 공백 부분 설정 
        if (cancelButtonImage != null)
            cancelButtonImage.alphaHitTestMinimumThreshold = 0.1f;

        if (attackButtonImage != null)
            attackButtonImage.alphaHitTestMinimumThreshold = 0.1f;

        //주인공 생각 말풍선 텍스트 초기화
        if (heroThinkText != null)
            heroThinkText.text = "";

        //정답 표시 텍스트 초기화
        if (answerText != null)
            answerText.text = "";
    }

    // 단어 버튼 클릭했을 때
    public void SelectWord(WordButton button, char letter)
    {
        if (isChecking)
            return;

        selectedButtons.Add(button);
        selectedLetters.Add(letter);
        
        if (audioSource != null && wordButtonSfx != null)
            audioSource.PlayOneShot(wordButtonSfx);

        // 선택한 글자를 주인공 생각 말풍선에 표시
        if (heroThinkText != null)
        {
            heroThinkText.text = new string(selectedLetters.ToArray());
        }
    }

    //선택된 단어 버튼들을 문자열로 반환
    public string GetAnswer()
    {
        return new string(selectedLetters.ToArray());
    }

    // 취소 버튼 클릭했을 때
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

    //말하기 버튼 클릭했을 때
    public void CheckAnswer()
    {        
        if (isChecking)
            return;

        if (ProblemManager.Instance == null || ProblemManager.Instance.currentProblem == null)
            return;

        if (audioSource != null && attackButtonSfx != null)
            audioSource.PlayOneShot(attackButtonSfx);
        
        StartCoroutine(CheckAnswerRoutine());
    }

    //정답, 오답 처리
    private IEnumerator CheckAnswerRoutine()
    {
        isChecking = true;

        if (Game3Manager.Instance != null && Game3Manager.Instance.attackButtonText != null)
            Game3Manager.Instance.attackButtonText.gameObject.SetActive(false);

        string playerAnswer = GetAnswer();

        //문제의 정답에서 공백 제거
        string correctAnswer = ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        //정답일 때
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("정답!");

            if (domitgirl != null)
                domitgirl.SetActive(false);

            if (domitgirlAngry != null)
            {
                domitgirlAngry.SetActive(true);

                Animator animator = domitgirlAngry.GetComponent<Animator>();

                if (animator != null)
                    animator.Play("DomitgirlAngry", 0, 0f);
            }

            if (audioSource != null && correctSfx != null)
                audioSource.PlayOneShot(correctSfx);

            selectedButtons.Clear();
            selectedLetters.Clear();

            if (heroThinkText != null)
                heroThinkText.text = "";

            //주인공 생각 말풍선 숨김
            if (heroThinkBubble != null)
                heroThinkBubble.SetActive(false);

            //주인공 기본 말풍선 표시
            if (heroBubble != null)
                heroBubble.SetActive(true);

            //주인공 기본 말풍선 안에 정답 표시
            if (heroTextText != null)
                heroTextText.text = ProblemManager.Instance.currentProblem.answer;

            if (Game3Manager.Instance != null && Game3Manager.Instance.enemy != null)
            {
                Game3Manager.Instance.enemy.Damage(20);
            }

            // 게임이 끝났으면
            if (Game3Manager.Instance != null && Game3Manager.Instance.IsGameEnded)
            {
                isChecking = false;
                yield break;
            }

            //1초 동안 정답 표시
            yield return new WaitForSeconds(1f);

            if (domitgirlAngry != null)
                domitgirlAngry.SetActive(false);

            if (domitgirl != null)
                domitgirl.SetActive(true);

            if (heroBubble != null)
                heroBubble.SetActive(false);

            if (heroThinkBubble != null)
                heroThinkBubble.SetActive(true);

            if (Game3Manager.Instance != null && !Game3Manager.Instance.IsGameEnded && Game3Manager.Instance.attackButtonText != null)
            {
                Game3Manager.Instance.attackButtonText.gameObject.SetActive(true);
            }

            //다음 문제 생성
            ProblemManager.Instance.NextProblem();
        }
        else    //오답일 때
        {
            Debug.Log("오답!");

            if (audioSource != null && wrongSfx != null)
                audioSource.PlayOneShot(wrongSfx);

            if (hintText != null)
                hintText.SetActive(true);

            //정답 문장 표시
            if (answerText != null)
            {
                answerText.gameObject.SetActive(true);
                answerText.text = "정답 : <color=red>" + ProblemManager.Instance.currentProblem.answer + "</color>";
            }

            // 3초 동안 정답 표시
            yield return new WaitForSeconds(3f);

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

            if (Game3Manager.Instance != null && !Game3Manager.Instance.IsGameEnded)
            {
                if (Game3Manager.Instance.attackButtonText != null)
                    Game3Manager.Instance.attackButtonText.gameObject.SetActive(true);
                
                ProblemManager.Instance.NextProblem();
            }
        }
            isChecking = false;
    }
}