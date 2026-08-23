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
    ChangeStreet,
    ChangeCafe,
    ChangeToBackground
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;

    [TextArea(3, 5)]
    public string text;

    public Sprite portrait;

    public DialogueEvent dialogueEvent;

    public bool isChoice;

    public int maxNoCount = 10;
}