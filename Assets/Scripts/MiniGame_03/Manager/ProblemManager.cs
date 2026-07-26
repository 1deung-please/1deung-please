using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ProblemManager : MonoBehaviour
{
    public TMP_Text dialogText;
    public static ProblemManager Instance;
    public List<Problem> problems = new();
    public Problem currentProblem;

    void Start()
    {
        NextProblem();
    }
    
    private void Awake()
    {
        Instance = this;
        CreateProblems();
    }

    void CreateProblems()
    {
        problems.Add(new Problem()
        {
            dialogue = "굿 한번 하셔야겠어요.",
            answer = "돈 없어요",
            shuffled = "없요돈어"
        });

        problems.Add(new Problem()
        {
            dialogue = "조상님이 노하셨네요.",
            answer = "기도 드릴게요",
            shuffled = "드요기릴게도"
        });

        problems.Add(new Problem()
        {
            dialogue = "액운이 가득합니다.",
            answer = "부적 사세요",
            shuffled = "세요적사부"
        });

        problems.Add(new Problem()
        {
            dialogue = "큰일 나실 팔자예요.",
            answer = "살려 주세요",
            shuffled = "주세살요려"
        });

        problems.Add(new Problem()
        {
            dialogue = "신령님께서 부르십니다.",
            answer = "안 갈래요",
            shuffled = "갈안래요"
        });
    }

    public void NextProblem()
    {
        int random = Random.Range(0, problems.Count);

       currentProblem = problems[random];

        // 대사 표시
        dialogText.text = currentProblem.dialogue;

        WordSpawner.Instance.SpawnWords(currentProblem.shuffled);
    }
}