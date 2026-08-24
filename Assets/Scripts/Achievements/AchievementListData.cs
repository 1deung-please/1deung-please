using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementInfo
{
    public int id;
    public string title;
    [TextArea]
    public string description; // 팝업에 표시할 달성 조건 텍스트 
    public Sprite badge;       // 해금됐을 때 표시할 개별 뱃지 이미지
}

[CreateAssetMenu(fileName = "AchievementList", menuName = "Game/AchievementList")]
public class AchievementListData : ScriptableObject
{
    public List<AchievementInfo> achievements;
}