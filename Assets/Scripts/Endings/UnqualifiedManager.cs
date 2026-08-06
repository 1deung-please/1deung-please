using System.Collections;
using UnityEngine;
using TMPro;

public class Ending_AManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreBoard;

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
        yield return Dialogue(
            "그래... 처음 보는구나."
        );


        yield return Dialogue(
            "내가 바로 네 조상이다."
        );


        yield return Dialogue(
            "내가 널 참 오랫동안 지켜보고 있었지...\n" +
            "갓난아기일 때부터 회사에 치이는 지금까지..."
        );


        yield return Dialogue(
            "얼마나 고생이 많았느냐.\n" +
            "난 널 도와주러 온 사람이야."
        );


        yield return Dialogue(
            "그럼 어디,\n" +
            "지난 시간동안 얼마나 공덕을 쌓아왔는지 볼까."
        );


        // 점수판 등장
        ShowScore();


        yield return Dialogue(
            "....."
        );


        yield return Dialogue(
            "..............."
        );

        yield return Dialogue(
            "................................................"
        );

        yield return Dialogue(
            "정말 보잘 것 없구나.."
        );

        yield return Dialogue(
            "오랫동안 봐 왔지만, 학생 때부터 지금까지 참 한결같이 성적이 안 좋구나. 꾸준하네..."
        );

        yield return Dialogue(
            "플레이를 한 건 맞느냐? 혹, 회사나 학교에서 몰폰 중이라 플레이를 제대로 못 하였던 것이냐?" 
        );

        yield return Dialogue(
            "흠..... 볼 것도 없구나. 돌아가서 다시 공덕을 쌓고 오거라!"
        );
    }

    IEnumerator Dialogue(string text)
    {
        dialogueText.text = "";

        isTyping = true;
        skipTyping = false;

        clickCoroutine = StartCoroutine(CheckClick());

        foreach(char c in text)
        {
            if(skipTyping)
            {
                dialogueText.text = text;
                break;
            }

            dialogueText.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if(clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
        }

        skipTyping = false;

        yield return new WaitUntil(
            () => Input.GetMouseButtonDown(0)
        );
    }

    IEnumerator CheckClick()
    {
        while(isTyping)
        {
            if(Input.GetMouseButtonDown(0))
            {
                skipTyping = true;
            }

            yield return null;
        }
    }

    // 점수판 출력
    void ShowScore()
    {
        scoreBoard.SetActive(true);


        scoreText.text =
            "공덕 점수: " + gameData.meritPoint;
    }
}