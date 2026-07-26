using System.Collections.Generic;
using UnityEngine;

public class Game3Manager : MonoBehaviour
{
    public static Game3Manager Instance;
    public EnemyManager enemy;
    private List<char> selectedChars = new List<char>();

    private void Awake()
    {
        Instance = this;
    }

    // 글자 선택
    public void SelectChar(char c)
    {
        selectedChars.Add(c);

        Debug.Log(new string(selectedChars.ToArray()));
    }

    // 공격 버튼
    public void Attack()
    {
        string playerAnswer = new string(selectedChars.ToArray());

        string correctAnswer =
            ProblemManager.Instance.currentProblem.answer.Replace(" ", "");

        if (playerAnswer == correctAnswer)
        {
            Debug.Log("정답");
            enemy.Damage(20);

            // 다음 문제
            ProblemManager.Instance.NextProblem();
        }
        else
        {
            Debug.Log("오답");
        }

        selectedChars.Clear();
    }
}