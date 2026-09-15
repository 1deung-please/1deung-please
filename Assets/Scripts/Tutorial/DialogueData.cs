using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;
}

public enum DialogueEvent
{
    None,
    MoveLobby,
    StartTimer,
    FadeOut,
    Achievement,
    ChangeStreet,
    ChangeCafe,
    ChangeToTimer,
    ChangeToTutorial_bus_stop,
    ChangeToTutorial_underground_shopping_center,
    ChangeToTutorial_cafe,
    ChangeToTutorial_bag,
    ChangeToStreet_Normal
}

//애니메이션 종류를 고르는 목록
public enum DialogueAnimEvent
{
    None,      
    PlayerAnim,
    DobmitgirlAnim,
    Dobmitgir_angry_dialAnim
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;

    [TextArea(3, 5)]
    
    public string text;
    public Sprite portrait;
    public DialogueEvent dialogueEvent;
    public DialogueAnimEvent animEvent; 

    public bool isChoice;

    [Header("Position Settings")]
    public bool isNormalDialogue;
    public int maxNoCount = 10;
}