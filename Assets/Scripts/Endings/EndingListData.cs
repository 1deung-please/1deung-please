using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndingInfo
{
    public string id;      // "æ‚∆≈«—º”º¿" µÓ
    public string title;   // «•Ω√µ… ¿Ã∏ß
    public string sceneName; // ¿Áª˝«“ æ¿ ¿Ã∏ß
    public Sprite badge;
}

[CreateAssetMenu(fileName = "EndingList", menuName = "Game/EndingList")]
public class EndingListData : ScriptableObject
{
    public List<EndingInfo> endings;
}