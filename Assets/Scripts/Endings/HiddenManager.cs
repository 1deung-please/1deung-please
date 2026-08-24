using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HiddenManager : MonoBehaviour
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
    [SerializeField] private Sprite dobmitgirl;
    [SerializeField] private Sprite narration;

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
        yield return Dialogue("???", "엔딩 4개를 다 보셨군요!", narration);

        yield return Dialogue("주인공", "플레이 해주셔서 진심으로 감사합니다!", player);

        yield return Dialogue("주인공", "플레이어님은 게임에서 뿐만 아니라, 진정한 귀인이세요!", player);

        yield return Dialogue("주인공", "여기, 열심히 플레이 해주신 당신께 주는 선물입니다.", player);

        yield return Dialogue("조상님", "사실 이 게임에서 가장 얻기 어려운 건 1등 당첨이 아니라...", ancestorGod);

        yield return Dialogue("조상님", "엔딩 4개를 모두 보는 것이었답니다!", ancestorGod);

        yield return Dialogue("주인공", "그러니 오늘만큼은 당당하게 말하세요.", player);

        yield return Dialogue("주인공", "나는 운 좋은 사람이다!", player);

        yield return Dialogue("조상님", "언젠가 현실에서도 좋은 일이 찾아오길 바랍니다.", ancestorGod);

        yield return Dialogue("도믿걸", "그리고...", dobmitgirl);

        yield return Dialogue("도믿걸", "다음에 복권을 긁게 된다면,", dobmitgirl);

        yield return Dialogue("주인공", "조상님 대신 저희가 응원하고 있을게요!", player);

        yield return Dialogue("전원", " 1등 되게 해주세요!!", narration);
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