using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndingInfo
{
    public string endingId;        // AchievementManager.OnEndingConfirmed의 switch문과 같은 문자열
                                   // ("얄팍한속셈", "자격미달", "절반의성공", "진정한귀인", "히든")
    public int linkedAchievementId; // 이 엔딩과 매칭되는 업적 ID (14~18) - 해금 여부 판정에 재사용
    public string title;            // 엔딩 이름
    public Sprite unlockedIcon;     // 해금 시 표시할 지폐 이미지
    public Sprite lockedIcon;       // 잠겼을 때 표시할 이미지 (엔딩마다 다름)
    public string sceneName;        // 다시보기 버튼 클릭 시 SceneLoader로 이동할 씬 이름
}

[CreateAssetMenu(fileName = "EndingList", menuName = "Game/EndingList")]
public class EndingListData : ScriptableObject
{
    public List<EndingInfo> endings;
}