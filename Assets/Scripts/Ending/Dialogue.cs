using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string speaker;

    [TextArea(2,5)]
    public string text;
}