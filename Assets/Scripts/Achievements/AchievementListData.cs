using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementInfo
{
    public int id;
    public string title;
    public Sprite badge;
}

[CreateAssetMenu(fileName = "AchievementList", menuName = "Game/AchievementList")]
public class AchievementListData : ScriptableObject
{
    public List<AchievementInfo> achievements;
}