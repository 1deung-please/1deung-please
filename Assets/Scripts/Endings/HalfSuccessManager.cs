using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HalfSuccessManager : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
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

        yield return Dialogue(
            "조상님",
            "내가 널 참 오랫동안 지켜보고 있었지...\n" +
            "갓난아기일 때부터 회사에 치이는 지금까지...",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "얼마나 고생이 많았느냐.\n" +
            "난 널 도와주러 온 사람이야.",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "그럼 어디, 지난 시간동안 얼마나 공덕을 쌓아왔는지 볼까.",
            ancestorGod
        );

        yield return Dialogue("조상님", "흠... 어디 보자...", ancestorGod);

        yield return Dialogue("조상님", "참으로 애~매하구나!", ancestorGod);

        yield return Dialogue(
            "조상님",
            "열심히 안 한 건 아닌데, 그렇다고 눈물겹게 열심히 한 것도 아니고...",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "딱 주 5일 턱걸이로 출근 도장만 찍은 느낌이구나...",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "그래도 이 팍팍한 세상에 평타라도 친 게 어디냐.",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "네 성의를 봐서 대박 복권까지는 아니어도, 로또 3등 당첨권을 내려주마!",
            ancestorGod
        );

        yield return Dialogue(
            "조상님",
            "감질나느냐? 억울하면 다음엔 눈 딱 감고 풀악셀로 덕 한번 쌓아보거라!",
            ancestorGod
        );

        yield return Dialogue("조상님", "자, 리스폰 고고!", ancestorGod);
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
}