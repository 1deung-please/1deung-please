using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShallowManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreBoard;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite ancestorGod;
    [SerializeField] private Sprite player;
    
    private bool isTyping = false;
    private bool skipTyping = false;

    // 클릭을 한 번만 감지하기 위한 변수
    private bool clickRequested = false;

    private void Start()
    {
       scoreBoard.SetActive(false);

        if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(false);
        }

        StartCoroutine(EndingStart());
    }

    private void Update()
    {
        // 모든 클릭은 여기서만 감지
        if (Input.GetMouseButtonDown(0))
        {
            clickRequested = true;
        }
    }

    IEnumerator EndingStart()
    {
        yield return Dialogue("조상님", "그래... 처음 보는구나.", ancestorGod);

        yield return Dialogue("조상님", "내가 바로 네 조상이다.", ancestorGod);

        yield return Dialogue("조상님", "내가 널 참 오랫동안 지켜보고 있었지...\n" + "갓난아기일 때부터 회사에 치이는 지금까지...", ancestorGod);

        yield return Dialogue("조상님", "얼마나 고생이 많았느냐.\n" + "난 널 도와주러 온 사람이야.", ancestorGod);

        yield return Dialogue("조상님", "그럼 어디, 지난 시간동안 얼마나 공덕을 쌓아왔는지 볼까.", ancestorGod);

        yield return ShowScore();

        yield return Dialogue("조상님", "흠... 점수는 괜찮고, 그래 꽤 잘 쌓아왔구나. 그래그래", ancestorGod);

        yield return Dialogue("조상님", "뭐얏!!!!", ancestorGod);

        string mostPlayedGame = GetMostPlayedGame();

        yield return Dialogue("조상님", "네 이녀석! 지금까지 진심으로 공덕을 쌓은 것이 아니라 오로지 돈만 바라보며 공덕을 쌓은 것이구나!!!", ancestorGod);

        yield return Dialogue("조상님", "가장 공덕 쌓기 쉬운 " + mostPlayedGame + "로 공덕 쌓기만 했어!!!!!!", ancestorGod);

        yield return Dialogue("조상님", "너는 선행을 위한 선행을 한 것이 아니라\n" + "오로지 돈만 보고 일을 한 것이로구나!", ancestorGod);

        yield return Dialogue("조상님", "썩 꺼지거라!\n" + "그리고 다시 진심을 다해 공덕을 쌓아오거라!!!", ancestorGod);
    }

    IEnumerator Dialogue(string speaker, string text, Sprite portrait)
    {
        // 이름 변경
        if (nameText != null)
        {
            nameText.text = speaker;
        }

        // 사진 변경
        if (portraitImage != null)
        {
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        dialogueText.text = "";

        isTyping = true;
        skipTyping = false;

        // 이전 클릭 제거
        clickRequested = false;

        // 타이핑
        foreach (char c in text)
        {
            // 타이핑 중 클릭하면 즉시 전체 대사 표시
            if (clickRequested)
            {
                dialogueText.text = text;

                // 이 클릭은 "타이핑 스킵"에 사용했으므로 제거
                clickRequested = false;

                break;
            }

            dialogueText.text += c;

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        // 타이핑이 끝난 뒤 혹시 남아있는 클릭 제거
        clickRequested = false;

        // 대사가 모두 나온 후
        // 새로운 클릭을 기다림
        yield return new WaitUntil(() => clickRequested);

        // 이 클릭은 다음 대사로 넘어가는 데 사용
        clickRequested = false;
    }

    IEnumerator ShowScore()
    {
        // 스코어보드 표시
        scoreBoard.SetActive(true);
        scoreText.text = "공덕 점수: " + gameData.meritPoint;

        // 이전 클릭 제거
        clickRequested = false;

        // 새로운 클릭을 기다림
        yield return new WaitUntil(() => clickRequested);

        // 클릭하면 스코어보드 숨김
        clickRequested = false;
        scoreBoard.SetActive(false);
    }

    string GetMostPlayedGame()
    {
        int max = gameData.playCount[0];
        int index = 0;

        for (int i = 1; i < gameData.playCount.Length; i++)
        {
            if (gameData.playCount[i] > max)
            {
                max = gameData.playCount[i];
                index = i;
            }
        }

        switch (index)
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