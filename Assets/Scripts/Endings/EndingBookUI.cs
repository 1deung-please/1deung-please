using UnityEngine;

public class EndingBookUI : MonoBehaviour
{
    public Transform slotParent;
    public GameObject slotPrefab;
    public EndingListData endingList;

    void OnEnable()
    {
        RefreshBook();
    }

    void RefreshBook()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (var info in endingList.endings)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            var slot = slotObj.GetComponent<EndingSlotUI>();
            bool unlocked = EndingStorage.IsUnlocked(info.id);
            slot.Setup(info, unlocked);
        }
    }
}