using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementInfo
{
    public int id;
    public string title;
    [TextArea]
    public string description; // 팝업에 표시할 달성 조건 텍스트 
    public Sprite badge;       // 도감에 보이는 메달 아이콘
    public Sprite popupImage;  // 팝업창에 뜨는 배경 이미지
}

[CreateAssetMenu(fileName = "AchievementList", menuName = "Game/AchievementList")]
public class AchievementListData : ScriptableObject
{
    public List<AchievementInfo> achievements;
}