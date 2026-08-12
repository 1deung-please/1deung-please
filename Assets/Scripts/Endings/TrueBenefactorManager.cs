using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrueBenefactorManager : MonoBehaviour
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
    [SerializeField] private Sprite dobmitgirl;

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

        yield return Dialogue("조상님", "어이쿠야!!!", ancestorGod);

        yield return Dialogue("조상님", "매, 맵다 매워! 점수판에서 불이 나는구나!!!", ancestorGod);

        yield return Dialogue("조상님", "대박이다, 대박이야! 우리 가문에 드디어 전설의 '미친 재능'이 태어났었구나!", ancestorGod);

        yield return Dialogue("조상님", gameData.meritPoint + "점이라니, 저승 공덕 인플루언서 랭킹 1위 각이로다!", ancestorGod);

        yield return Dialogue("조상님", "네 덕에 내가 저승 노인정에서 \"내 후손이 이 정도다!\"라며 어깨뽕을 우주까지 세우고 다닐 수 있게 됐다!", ancestorGod);

        yield return Dialogue("조상님", "고맙다, 내 새끼!!!", ancestorGod);

        yield return Dialogue("주인공", "저... 조상님? 그럼 저 정말 로또 1등 되는 건가요?", player);

        yield return Dialogue("조상님", "당연하다마다. 내가 누구냐, 네 조상 아니더냐...", ancestorGod);

        yield return Dialogue("주인공", "...", player);

        yield return Dialogue("주인공", "....", player);

        yield return Dialogue("주인공", "끙...", player);

        yield return Dialogue("조상님", "어린 나이에 집안에 빨간 딱지가 붙어 펑펑 울 때도,", ancestorGod);

        yield return Dialogue("조상님", "돈이 없어 삼각김밥 하나로 하루를 버틸 때도...", ancestorGod);

        yield return Dialogue("조상님", "그리고 매일 회사에서 치이며 '다 때려치고 싶다'고 소리 없는 비명을 지를때도...", ancestorGod);

        yield return Dialogue("조상님", "난 늘 네 곁에서 가슴을 쥐어짜며 함께 울고 있었다.", ancestorGod);

        yield return Dialogue("조상님", "이 미련한 녀석아, 네가 그동안 얼마나 팍팍하고 외롭게 살아왔는지 내가 다 안다.", ancestorGod);

        yield return Dialogue("조상님", "오죽하면 그 길바닥에서 도믿걸이 말을 걸었는데도 덥석 따라왔겠느냐...", ancestorGod);

        yield return Dialogue("주인공", "...네? 도믿걸요? 그걸 조상님이 어떻게...", player);

        yield return Dialogue("도믿걸", "제가원래이런말잘안하는데요, 귀인님 인상이 참 좋으십니다? 후후.", dobmitgirl);

        yield return Dialogue("주인공", "도... 도믿걸?! 당신이 왜 ?!", player);

        yield return Dialogue("조상님", "네가 도통 잠을 안 자 조상 꿈을 안 꿔주니,", ancestorGod);

        yield return Dialogue("조상님", "내가 답답해서 저승 법률 위반해가며 직접 지상으로 다이렉트 중계를 내려간 것이지!", ancestorGod);

        yield return Dialogue("조상님", "네 녀석에게 억지로라도 선행을 베풀게 해서, 떳떳하게 대박 복을 내릴 명분을 만들려고 말이다!", ancestorGod);

        yield return Dialogue("조상님", "근데 내 예상을 뛰어넘어 이렇게 완벽하게 공덕을 쌓아오다니...", ancestorGod);

        yield return Dialogue("조상님", "역시 내 핏줄다워! 장하다, 내 새끼!", ancestorGod);

        yield return Dialogue("조상님", "이제 그 눈물 젖은 삼각김밥과 꼰대 상사는 영원히 안녕이다!", ancestorGod);

        yield return Dialogue("조상님", "오냐! 내 감동의 눈물이앞을 가리는구나!", ancestorGod);

        yield return Dialogue("조상님", "저기 황금 카펫 깔린 환생의 문 보이느냐?", ancestorGod);

        yield return Dialogue("조상님", "당당하게 워킹해서 가거라!", ancestorGod);

        yield return Dialogue("조상님", "넌 다음 생... 아니, 이번 생의 당당한 주인공이니까! 웰컴 투 리치 라이프!!!", ancestorGod);
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