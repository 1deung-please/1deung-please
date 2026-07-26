using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public TMP_Text answerText;
    private bool isPenalty = false;
    public static AnswerManager Instance;
    public Transform answerPanel;
    private List<WordButton> selectedWords = new();

    private void Awake()
    {
        Instance = this;
    }

    public void SelectWord(WordButton word)
    {
        if (selectedWords.Contains(word))
            return;

        selectedWords.Add(word);
        word.transform.SetParent(answerPanel, false);
    }

    public void RemoveWord(WordButton word)
    {
        if (!selectedWords.Contains(word))
            return;

        selectedWords.Remove(word);
        word.ReturnToOrigin();
    }

    public string GetAnswer()
    {
        string result = "";

        foreach (WordButton word in selectedWords)
            result += word.GetWord();

        return result;
    }

    public void Clear()
    {
        foreach (WordButton word in new List<WordButton>(selectedWords))
            word.ReturnToOrigin();

        selectedWords.Clear();
    }

    public void CheckAnswer()
    {
        // 내가 만든 답
        string myAnswer = GetAnswer().Replace(" ", "");

        // 정답
        string correctAnswer = ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        if (myAnswer == correctAnswer)
        {
            Debug.Log("정답!");

            FindFirstObjectByType<EnemyManager>().Damage(20);
            Clear();
            ProblemManager.Instance.NextProblem();
        }
        else
        {
            StartCoroutine(Penalty());
        }
    }

    IEnumerator Penalty()
    {
        if (isPenalty)
            yield break;

        isPenalty = true;

        answerText.gameObject.SetActive(true);
        answerText.text = "정답 : " + ProblemManager.Instance.currentProblem.answer;

        yield return new WaitForSeconds(3f);

        answerText.gameObject.SetActive(false);

        Clear();

        ProblemManager.Instance.NextProblem();

        isPenalty = false;
    }
}