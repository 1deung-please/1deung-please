using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public TMP_Text answerText;
    public GameObject hintText;
    private bool isPenalty = false;
    public static AnswerManager Instance;
    public Transform answerPanel;
    private List<WordButton> selectedWords = new();

    [Header("말풍선")]
    public GameObject heroThinkText;
    public GameObject heroText;

    public TMP_Text thinkAnswerText;
    public TMP_Text speechAnswerText;

    private void Awake()
    {
        Instance = this;

        if (hintText != null)
            hintText.SetActive(false);

        if (heroThinkText != null)
            heroThinkText.SetActive(false);

        if (heroText != null)
            heroText.SetActive(false);
    }

    public void SelectWord(WordButton word)
    {
        if (Game3Manager.Instance != null &&
            Game3Manager.Instance.IsGameEnded)
                return;

        if (isPenalty)
        return;

        if (selectedWords.Contains(word))
            return;

        selectedWords.Add(word);

        word.Select();

        if (thinkAnswerText != null)
            thinkAnswerText.text = GetAnswer();
    }

    public void RemoveWord(WordButton word)
    {
        if (!selectedWords.Contains(word))
            return;

        selectedWords.Remove(word);

        thinkAnswerText.text = GetAnswer();
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
        foreach (WordButton word in selectedWords)
        {
            if (word != null)
                word.ResetWord();
        }

        selectedWords.Clear();

        if (thinkAnswerText != null)
            thinkAnswerText.text = "";

        if (speechAnswerText != null)
            speechAnswerText.text = "";
    }

    public void CheckAnswer()
{
    if (isPenalty)
        return;

    if (Game3Manager.Instance != null &&
        Game3Manager.Instance.IsGameEnded)
        return;

    string spokenAnswer = GetAnswer();

    if (heroThinkText != null)
        heroThinkText.SetActive(false);

    if (heroText != null)
        heroText.SetActive(true);

    if (speechAnswerText != null)
        speechAnswerText.text = spokenAnswer;

    string myAnswer = spokenAnswer.Replace(" ", "");

    string correctAnswer =
        ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

    if (myAnswer == correctAnswer)
    {
        Debug.Log("정답!");

        StartCoroutine(CorrectAnswer());
    }
    else
    {
        StartCoroutine(Penalty());
    }
}

private void ResetBubble()
{
    if (heroThinkText != null)
        heroThinkText.SetActive(true);

    if (heroText != null)
        heroText.SetActive(false);

    if (thinkAnswerText != null)
        thinkAnswerText.text = "";

    if (speechAnswerText != null)
        speechAnswerText.text = "";
}

    IEnumerator CorrectAnswer()
    {
        yield return new WaitForSeconds(1f);

        EnemyManager enemyManager =
            FindFirstObjectByType<EnemyManager>();

        if (enemyManager != null)
            enemyManager.Damage(20);


        Clear();


        if (Game3Manager.Instance != null &&
            !Game3Manager.Instance.IsGameEnded)
        {
            ProblemManager.Instance.NextProblem();

            ResetBubble();
        }
    }

    IEnumerator Penalty()
{
    if (isPenalty)
        yield break;

    isPenalty = true;

    if(hintText != null)
        hintText.SetActive(true);

    answerText.text =
        "정답 : " + ProblemManager.Instance.currentProblem.answer;

    yield return new WaitForSeconds(3f);

    if (hintText != null)
        hintText.SetActive(false);


    Clear();

    if (Game3Manager.Instance != null &&
        !Game3Manager.Instance.IsGameEnded)
    {
        ProblemManager.Instance.NextProblem();

        ResetBubble();
    }

    isPenalty = false;
}
}