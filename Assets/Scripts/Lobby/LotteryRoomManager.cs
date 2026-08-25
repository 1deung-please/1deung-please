using UnityEngine;

public class LotteryRoomManager : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private ScratchLotteryManager scratchLotteryManager;

    private void Start()
    {
        if (dialogueManager != null && scratchLotteryManager != null)
            dialogueManager.OnDialogueFinished += scratchLotteryManager.ShowLottery;
    }

    private void OnDestroy()
    {
        if (dialogueManager != null && scratchLotteryManager != null)
            dialogueManager.OnDialogueFinished -= scratchLotteryManager.ShowLottery;
    }
}