using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ProblemManager : MonoBehaviour
{
    public static ProblemManager Instance;

    public TMP_Text dialogText;

    public List<Problem> problems = new List<Problem>();
    public Problem currentProblem;

    // 아직 이번 회차에서 나오지 않은 문제 번호
    private List<int> remainingProblemIndexes = new List<int>();

    private void Awake()
    {
        Instance = this;

        CreateProblems();
        ResetProblemIndexes();
    }

    private void Start()
    {
        //NextProblem();
    }

    private void CreateProblems()
    {
        problems.Clear();

        problems.Add(new Problem()
        {
            dialogue = "잠시 시간 괜찮으세요?",
            answer = "일분도없어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "설문조사 하나만요!",
            answer = "못하겠어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "인상이 정말 좋네요.",
            answer = "성형했습니다"
        });

        problems.Add(new Problem()
        {
            dialogue = "잠깐 이야기만…",
            answer = "다음에할게요"
        });

        problems.Add(new Problem()
        {
            dialogue = "좋은 기회입니다.",
            answer = "아돈노코리안"
        });

        problems.Add(new Problem()
        {
            dialogue = "5분만 투자하세요!",
            answer = "버스와서요"
        });

        problems.Add(new Problem()
        {
            dialogue = "우연이 아닙니다!",
            answer = "우연입니다"
        });

        problems.Add(new Problem()
        {
            dialogue = "큰 기회를 놓쳐요!",
            answer = "놓쳐도됩니다"
        });

        problems.Add(new Problem()
        {
            dialogue = "오늘 운세가…",
            answer = "오하아사봤어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "진리를 찾고…",
            answer = "관심없어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "좋은 말씀 전할게요",
            answer = "필요없어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "혹시 고민 있으세요?",
            answer = "그런거없어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "퍼스널컬러 체험해보고 가세요",
            answer = "겨쿨입니다"
        });

        problems.Add(new Problem()
        {
            dialogue = "나만의 향수 만들어보세요",
            answer = "제가꽃이라서"
        });

        problems.Add(new Problem()
        {
            dialogue = "배우에 관심 있으세요?",
            answer = "이미데뷔했어요"
        });

        problems.Add(new Problem()
        {
            dialogue = "혹시 운명을 믿으시나요?",
            answer = "엄마가기다려요"
        });

        problems.Add(new Problem()
        {
            dialogue = "무료로 상담해 드리고 있어요.",
            answer = "가봐야해요"
        });
    }

    private void ResetProblemIndexes()
    {
        remainingProblemIndexes.Clear();

        for (int i = 0; i < problems.Count; i++)
        {
            remainingProblemIndexes.Add(i);
        }
    }

    public void NextProblem()
    {
        // 모든 문제를 한 번씩 사용했으면 다시 전체 문제 사용 가능
        if (remainingProblemIndexes.Count == 0)
        {
            ResetProblemIndexes();
        }

        // 아직 나오지 않은 문제 중 하나를 랜덤 선택
        int randomPosition = Random.Range(0, remainingProblemIndexes.Count);
        int problemIndex = remainingProblemIndexes[randomPosition];

        currentProblem = problems[problemIndex];

        // 이번 회차에서는 다시 나오지 않도록 제거
        remainingProblemIndexes.RemoveAt(randomPosition);

        // 대사 표시
        dialogText.text = currentProblem.dialogue;

        // 정답 공백 제거
        string answerWithoutSpaces = currentProblem.answer.Replace(" ", "");

        // 정답 글자 순서를 매번 랜덤으로 섞기
        string shuffledAnswer = ShuffleText(answerWithoutSpaces);

        WordSpawner.Instance.SpawnWords(shuffledAnswer);
    }

    private string ShuffleText(string text)
    {
        List<char> characters = new List<char>(text);

        for (int i = characters.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            char temp = characters[i];
            characters[i] = characters[randomIndex];
            characters[randomIndex] = temp;
        }

        string shuffled = new string(characters.ToArray());

        // 섞었는데 정답과 완전히 같으면 다시 섞기
        if (shuffled == text && text.Length > 1)
        {
            return ShuffleText(text);
        }

        return shuffled;
    }
}