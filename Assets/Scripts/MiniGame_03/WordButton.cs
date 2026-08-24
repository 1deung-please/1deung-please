using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    [Header("UI")]
    public Image buttonImage;
    public TMP_Text wordText;

    [Header("Button Sprite")]
    public Sprite normalSprite;
    public Sprite blankSprite;

    private char letter;
    private bool isSelected = false;

    public char Letter => letter;

    private void Awake()
    {
        if (buttonImage != null)
        {
            buttonImage.alphaHitTestMinimumThreshold = 0.1f;
        }
    }

    public void Initialize(char newLetter)
    {
        letter = newLetter;
        isSelected = false;

        buttonImage.sprite = normalSprite;

        wordText.gameObject.SetActive(true);
        wordText.text = newLetter.ToString();
    }

    public void SetEmpty()
    {
        letter = '\0';
        isSelected = true;

        buttonImage.sprite = blankSprite;

        wordText.text = "";
        wordText.gameObject.SetActive(false);
    }

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