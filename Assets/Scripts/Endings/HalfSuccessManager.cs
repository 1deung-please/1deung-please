using System.Collections;
using UnityEngine;
using TMPro;

public class HalfSuccessManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreBoard;
    [SerializeField] private TextMeshProUGUI nameText; 

    private bool isTyping = false;
    private bool skipTyping = false;
    private Coroutine clickCoroutine;

    private void Start()
    {
        scoreBoard.SetActive(false);

        StartCoroutine(EndingStart());
    }

    IEnumerator EndingStart()
    {
        yield return Dialogue("조상님", "그래... 처음 보는구나.");
        yield return Dialogue("조상님", "내가 바로 네 조상이다.");
        yield return Dialogue("조상님", "내가 널 참 오랫동안 지켜보고 있었지...\n" +
                                     "갓난아기일 때부터 회사에 치이는 지금까지...");
        yield return Dialogue("조상님", "얼마나 고생이 많았느냐.\n" +
                                     "난 널 도와주러 온 사람이야.");
        yield return Dialogue("조상님", "그럼 어디,\n" +
                                     "지난 시간동안 얼마나 공덕을 쌓아왔는지 볼까.");

        ShowScore();

        yield return Dialogue("조상님", "흠... 어디 보자...");
        yield return Dialogue("조상님", "참으로 애~매하구나!");
        yield return Dialogue("조상님", "열심히 안 한 건 아닌데, 그렇다고 눈물겹게 열심히 한 것도 아니고...");
        yield return Dialogue("조상님", "딱 주 5일 턱걸이로 출근 도장만 찍은 느낌이구나...");
        yield return Dialogue("조상님", "그래도 이 팍팍한 세상에 평타라도 친 게 어디냐.");
        yield return Dialogue("조상님", "네 성의를 봐서 대박 복권까지는 아니어도, 로또 3등 당첨권을 내려주마!");
        yield return Dialogue("조상님", "감질나느냐? 억울하면 다음엔 눈 딱 감고 풀악셀로 덕 한번 쌓아보거라!");
        yield return Dialogue("조상님", "자, 리스폰 고고!");

    }

    IEnumerator Dialogue(string speaker, string text)
    {
        // 이름 텍스트 변경
        if (nameText != null)
        {
            nameText.text = speaker;
        }

        dialogueText.text = "";

        isTyping = true;
        skipTyping = false;

        clickCoroutine = StartCoroutine(CheckClick());

        foreach (char c in text)
        {
            if (skipTyping)
            {
                dialogueText.text = text;
                break;
            }

            dialogueText.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
        }

        skipTyping = false;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    IEnumerator CheckClick()
    {
        while (isTyping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                skipTyping = true;
            }

            yield return null;
        }
    }

    void ShowScore()
    {
        scoreBoard.SetActive(true);
        scoreText.text = "공덕 점수: " + gameData.meritPoint;
    }
}