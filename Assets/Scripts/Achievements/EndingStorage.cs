using UnityEngine;

public static class EndingStorage
{
    public static bool IsUnlocked(string endingId) => PlayerPrefs.GetInt($"Ending_{endingId}", 0) == 1;

    public static void Unlock(string endingId)
    {
        PlayerPrefs.SetInt($"Ending_{endingId}", 1);
        PlayerPrefs.Save();
    }
}