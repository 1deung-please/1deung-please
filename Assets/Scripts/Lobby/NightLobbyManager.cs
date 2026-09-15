using System.Collections;
using UnityEngine;
using TMPro;

public class NightLobbyManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;   
    [SerializeField] private GameObject dialogueUI;      
    [SerializeField] private TextMeshProUGUI nameText;     
    [SerializeField] private TextMeshProUGUI dialogueText; 

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (nameText != null) nameText.gameObject.SetActive(false);
        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(true);
        }

        StartCoroutine(LobbyStart());
    }

    private IEnumerator LobbyStart()
    {
        yield return StartCoroutine(Dialogue("주인공", "(5분이 다 지났다... 도믿걸이 로또방에 들어오면 무슨 일이 일어난다고 했어...)"));

        yield return StartCoroutine(Dialogue("주인공", "(로또 한 번 사보자.)"));

        yield return null;

        EndDialogue();
    }

    private IEnumerator Dialogue(string speaker, string text)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueUI != null) dialogueUI.SetActive(true);

        if (nameText != null)
        {
            if (string.IsNullOrEmpty(speaker))
            {
                nameText.gameObject.SetActive(false);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                nameText.text = speaker;
            }
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
            bool isTyping = true;

            Coroutine typing = StartCoroutine(TypeText(text, () => isTyping = false));

            while (isTyping)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    StopCoroutine(typing);
                    dialogueText.text = text; 
                    isTyping = false;
                    yield return null;
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);

        while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        yield return null; 
    }

    private IEnumerator TypeText(string text, System.Action onComplete)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        onComplete?.Invoke();
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false);

        if (dialoguePanel != null)
        {
            CanvasGroup group = dialoguePanel.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }

            dialoguePanel.SetActive(false);
        }
    }
}