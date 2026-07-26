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
    if (Game3Manager.Instance != null &&
        Game3Manager.Instance.IsGameEnded)
        return;

    if (selectedWords.Contains(word))
        return;

    selectedWords.Add(word);

    word.MoveToAnswerPanel(answerPanel);
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
    if (isPenalty)
        return;

    if (Game3Manager.Instance != null &&
        Game3Manager.Instance.IsGameEnded)
        return;

    string myAnswer = GetAnswer().Replace(" ", "");
    string correctAnswer =
        ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

    if (myAnswer == correctAnswer)
    {
        Debug.Log("정답!");

        FindFirstObjectByType<EnemyManager>().Damage(20);
        Clear();

        if (Game3Manager.Instance != null &&
            !Game3Manager.Instance.IsGameEnded)
        {
            ProblemManager.Instance.NextProblem();
        }
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
    answerText.text =
        "정답 : " + ProblemManager.Instance.currentProblem.answer;

    yield return new WaitForSeconds(3f);

    answerText.gameObject.SetActive(false);

    Clear();

    if (Game3Manager.Instance != null &&
        !Game3Manager.Instance.IsGameEnded)
    {
        ProblemManager.Instance.NextProblem();
    }

    isPenalty = false;
}
}