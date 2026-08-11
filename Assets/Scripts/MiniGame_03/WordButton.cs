using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    public TMP_Text text;

    private string letter;
    private Transform originalParent;
    private int originalSiblingIndex;
    private GameObject placeholder;

    public void Initialize(string value)
    {
        letter = value;
        text.text = value;

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void Click()
    {
        if (AnswerManager.Instance != null)
            AnswerManager.Instance.SelectWord(this);
    }

    public string GetWord()
    {
        return letter;
    }

    public void Select()
    {
        if (placeholder != null)
            return;

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        placeholder = new GameObject(
            "WordPlaceholder",
            typeof(RectTransform),
            typeof(LayoutElement)
        );

        placeholder.transform.SetParent(originalParent, false);
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);

        RectTransform myRect = GetComponent<RectTransform>();
        LayoutElement layout = placeholder.GetComponent<LayoutElement>();

        layout.preferredWidth = myRect.rect.width;
        layout.preferredHeight = myRect.rect.height;
        layout.minWidth = myRect.rect.width;
        layout.minHeight = myRect.rect.height;
        layout.flexibleWidth = 0;
        layout.flexibleHeight = 0;

        gameObject.SetActive(false);
    }

    public void ResetWord()
    {
        if (placeholder != null)
        {
            Destroy(placeholder);
            placeholder = null;
        }

        gameObject.SetActive(true);
    }
}