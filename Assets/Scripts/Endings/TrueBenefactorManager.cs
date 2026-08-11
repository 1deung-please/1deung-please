using System.Collections;
using UnityEngine;
using TMPro;

public class TrueBenefactorManager : MonoBehaviour
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

        yield return Dialogue("조상님", "어이쿠야!!!");
        yield return Dialogue("조상님", "매, 맵다 매워! 점수판에서 불이 나는구나!!!");
        yield return Dialogue("조상님", "대박이다, 대박이야! 우리 가문에 드디어 전설의 '미친 재능'이 태어났었구나!");
        yield return Dialogue("조상님", gameData.meritPoint + "점이라니, 저승 공덕 인플루언서 랭킹 1위 각이로다!");
        yield return Dialogue("조상님", "네 덕에 내가 저승 노인정에서 \"내 후손이 이 정도다!\"라며 어깨뽕을 우주까지 세우고 다닐 수 있게 됐다!");
        yield return Dialogue("조상님", "고맙다, 내 새끼!!!");
        yield return Dialogue("주인공", "저... 조상님? 그럼 저 정말 로또 1등 되는 건가요?");
        yield return Dialogue("조상님", "당연하다마다. 내가 누구냐, 네 조상 아니더냐...");
    }

    IEnumerator Dialogue(string speaker, string text)
    {
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

        // 핵심: 클릭 잔상 제거를 위한 1프레임 대기
        yield return null; 

        // 사용자가 다시 클릭할 때까지 대기
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        
        // 연속 클릭 방지용 1프레임 대기
        yield return null; 
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