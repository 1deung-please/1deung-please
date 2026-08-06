using System.Collections;
using UnityEngine;
using TMPro;

public class ShortcutManager : MonoBehaviour
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
            "흠...\n" +
            "점수는 괜찮고, 그래 꽤 잘 쌓아왔구나. 그래그래"
        );


        yield return Dialogue(
            "뭐얏!!!!"
        );


        string mostPlayedGame = GetMostPlayedGame();


        yield return Dialogue(
            "네 이녀석!\n" +
            "지금까지 진심으로 공덕을 쌓은 것이 아니라\n" +
            "오로지 돈만 바라보며 공덕을 쌓은 것이구나!!!"
        );


        yield return Dialogue(
            "가장 공덕 쌓기 쉬운 " +
            mostPlayedGame +
            "로 공덕 쌓기만 했어!!!!!!"
        );


        yield return Dialogue(
            "너는 선행을 위한 선행을 한 것이 아니라\n" +
            "오로지 돈만 보고 일을 한 것이로구나!"
        );


        yield return Dialogue(
            "썩 꺼지거라!\n" +
            "그리고 다시 진심을 다해 공덕을 쌓아오거라!!!"
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
            "공덕 점수: " + gameData.TotalScore;
    }

    // 가장 많이 플레이한 게임 찾기
    string GetMostPlayedGame()
    {
        int max = gameData.playCount[0];

        int index = 0;


        for(int i = 1; i < gameData.playCount.Length; i++)
        {
            if(gameData.playCount[i] > max)
            {
                max = gameData.playCount[i];
                index = i;
            }
        }

        switch(index)
        {
            case 0:
                return "<이걸 안 비켜?>";

            case 1:
                return "<출격! 논리요새>";

            case 2:
                return "<주워줘, 쓰레기>";
        }


        return "";
    }
}