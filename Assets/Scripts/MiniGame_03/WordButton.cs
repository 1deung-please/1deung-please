using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    public TMP_Text text;

    private string letter;
    private Transform originalParent;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
    }

    public void Initialize(string value)
    {
        letter = value;
        text.text = value;

        originalParent = transform.parent;
    }

    public void Click()
    {
        AnswerManager.Instance.SelectWord(this);
    }

    public string GetWord()
    {
        return letter;
    }

    public void ReturnToOrigin()
    {
        transform.SetParent(originalParent, false);
    }
}