using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndingInfo
{
    public string id;      // "얄팍한속셈" 등
    public string title;   // 표시될 이름
    public string sceneName; // 재생할 씬 이름
    public Sprite badge;
}

[CreateAssetMenu(fileName = "EndingList", menuName = "Game/EndingList")]
public class EndingListData : ScriptableObject
{
    public List<EndingInfo> endings;
}