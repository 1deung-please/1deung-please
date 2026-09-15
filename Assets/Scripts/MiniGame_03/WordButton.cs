using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    [Header("UI")]
    public Image buttonImage;   //단어 버튼 이미지
    public TMP_Text wordText;   //단어 버튼 텍스트

    [Header("Button Sprite")]
    public Sprite normalSprite; //기본 단어 버튼 이미지
    public Sprite blankSprite;  //블랭크 단어 버튼 이미지

    private char letter;                //단어 버튼 글자
    public char Letter => letter;
    private bool isSelected = false;    

    private void Awake()
    {
        if (buttonImage != null)    //이미지의 투명 공백 부분은 클릭되지 않도록 설정
        {
            buttonImage.alphaHitTestMinimumThreshold = 0.1f;
        }
    }

    //단어 버튼 글자 설정
    public void Initialize(char newLetter)
    {
        letter = newLetter;
        isSelected = false;

        buttonImage.sprite = normalSprite;

        wordText.gameObject.SetActive(true);
        wordText.text = newLetter.ToString();
    }

    //7글자 이하인 문장일때 나머지 블랭크 설정
    public void SetEmpty()
    {
        letter = '\0';
        isSelected = true;

        buttonImage.sprite = blankSprite;

        wordText.text = "";
        wordText.gameObject.SetActive(false);
    }

    //단어 버튼 눌렀을 때
    public void Select()
    {
        if (isSelected)
            return;

        isSelected = true;

        if (AnswerManager.Instance != null)
        {
            AnswerManager.Instance.SelectWord(this, letter);
        }

        buttonImage.sprite = blankSprite;
        wordText.gameObject.SetActive(false);
    }

    //취소 버튼 눌렀을 때 복구 과정
    public void Restore()
    {
        if (letter == '\0')
            return;

        isSelected = false;

        buttonImage.sprite = normalSprite;

        wordText.gameObject.SetActive(true);
        wordText.text = letter.ToString();
    }
}