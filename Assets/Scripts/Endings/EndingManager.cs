using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData endingDialogue;

    void Start()
    {
        StartEndingDialogue();
    }

    public void StartEndingDialogue()
    {
        dialogueManager.StartDialogue(endingDialogue);
    }

    // 대사 끝나고 마지막에 호출될 함수 (DialogueData의 마지막 줄 이벤트로 연결하거나, DialogueManager.EndDialogue()에서 호출)
    public void FinishGame()
    {
        // 여기서 추가로 할 일이 있으면 (예: 엔딩 완료 표시, 다음 버튼 활성화 등)
    }
}