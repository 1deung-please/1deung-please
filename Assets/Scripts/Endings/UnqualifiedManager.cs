using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnqualifiedManager : MonoBehaviour
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

        // 스코어보드 등장
        yield return ShowScore();

        yield return Dialogue("조상님", ".....", ancestorGod);

        yield return Dialogue("조상님", "...............", ancestorGod);

        yield return Dialogue("조상님", "................................................", ancestorGod);

        yield return Dialogue("조상님", "정말 보잘 것 없구나..", ancestorGod);

        yield return Dialogue("조상님", "오랫동안 봐 왔지만, 학생 때부터 지금까지 참 한결같이 성적이 안 좋구나. 꾸준하네...", ancestorGod);

        yield return Dialogue("조상님", "플레이를 한 건 맞느냐? 혹, 회사나 학교에서 몰폰 중이라 플레이를 제대로 못 하였던 것이냐?", ancestorGod);

        yield return Dialogue("조상님", "흠..... 볼 것도 없구나. 돌아가서 다시 공덕을 쌓고 오거라!", ancestorGod);
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
}