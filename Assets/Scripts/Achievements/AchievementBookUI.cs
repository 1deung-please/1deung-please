using UnityEngine;

public class AchievementBookUI : MonoBehaviour
{
    public Transform slotParent;   // 슬롯들이 배치될 부모 
    public GameObject slotPrefab;  // AchievementSlot 프리팹
    public AchievementListData achievementList;

    void OnEnable()
    {
        RefreshBook();
    }

    void RefreshBook()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var info in achievementList.achievements)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            var slot = slotObj.GetComponent<AchievementSlotUI>();
            bool unlocked = AchievementStorage.IsUnlocked(info.id);
            slot.Setup(info, unlocked);
        }
    }
}