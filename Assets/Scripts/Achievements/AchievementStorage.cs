using UnityEngine;

public static class AchievementStorage
{
    public static bool IsUnlocked(int id) => PlayerPrefs.GetInt($"Achv_{id}", 0) == 1;

    public static void Unlock(int id)
    {
        PlayerPrefs.SetInt($"Achv_{id}", 1);
        PlayerPrefs.Save();
    }
}