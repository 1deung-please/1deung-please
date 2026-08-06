using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LotteryDialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject scratchButton;

    [Header("대사")]
    [SerializeField] private Dialogue[] dialogues;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;

    private bool isTyping = false;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private string currentSentence;

    void Start()
    {
        scratchButton.SetActive(false);
        ShowDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                FinishTyping();
            }
            else
            {
                NextDialogue();
            }
        }
    }

    void ShowDialogue()
    {
        speakerText.text = dialogues[currentIndex].speaker;

        currentSentence = dialogues[currentIndex].text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeDialogue(currentSentence));
    }

    void NextDialogue()
    {
        currentIndex++;

        if (currentIndex >= dialogues.Length)
        {
            scratchButton.SetActive(true); 

            enabled = false; 
            return;
        }

        ShowDialogue();
    }

    IEnumerator TypeDialogue(string text)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentSentence;
        isTyping = false;
    }

    public void StartScratch()
    {
        Debug.Log("버튼 클릭!");
        StartCoroutine(FadeToEnding());
    }

    IEnumerator FadeToEnding()
    {
        for (float a = 0; a <= 1; a += 0.1f)
        {
            fadeImage.color = new Color(1, 1, 1, a);

            Debug.Log(a);

            yield return new WaitForSeconds(0.2f);
        }
    }
}