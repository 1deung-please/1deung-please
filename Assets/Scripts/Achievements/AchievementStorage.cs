using UnityEngine;

public static class AchievementStorage
{
    public static bool IsUnlocked(int id) => PlayerPrefs.GetInt($"Achv_{id}", 0) == 1;

    public static void Unlock(int id)
    {
        PlayerPrefs.SetInt($"Achv_{id}", 1);
        PlayerPrefs.Save();
    }

    public static void ClearAllAchievements()
    {
        // 1번부터 18번까지의 업적 키(Achv_1 ~ Achv_18)를 삭제합니다.
        for (int i = 1; i <= 18; i++)
        {
            string key = $"Achv_{i}";
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("<color=red>[AchievementStorage] 모든 업적 데이터가 초기화되었습니다.</color>");
    }
}