using UnityEngine;

public static class PersistentStats
{
    public static int GetResetCycleCount() => PlayerPrefs.GetInt("ResetCycleCount", 0);

    public static void IncrementResetCycleCount()
    {
        int count = GetResetCycleCount() + 1;
        PlayerPrefs.SetInt("ResetCycleCount", count);
        PlayerPrefs.Save();

        if (count >= 5)
            AchievementManager.Instance.TryUnlockPublic(19);
    }
}