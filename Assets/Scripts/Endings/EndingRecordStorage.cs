using UnityEngine;

public static class EndingRecordStorage
{
    private const string KEY_TRUE_BENEFACTOR_HIGH_MERIT = "EndingRecord_TrueBenefactor_HighMerit";
    private const string KEY_SHALLOW_LAST_GAME_INDEX = "EndingRecord_Shallow_LastGameIndex";

    public static int GetTrueBenefactorMerit()
    {
        return PlayerPrefs.GetInt(KEY_TRUE_BENEFACTOR_HIGH_MERIT, 8500);
    }

    public static void SaveTrueBenefactorMerit(int currentMerit)
    {
        int highestMerit = GetTrueBenefactorMerit();
        if (currentMerit > highestMerit)
        {
            PlayerPrefs.SetInt(KEY_TRUE_BENEFACTOR_HIGH_MERIT, currentMerit);
            PlayerPrefs.Save();
        }
    }

    public static int GetShallowGameIndex()
    {
        return PlayerPrefs.GetInt(KEY_SHALLOW_LAST_GAME_INDEX, 0);
    }

    public static void SaveShallowGameIndex(int gameIndex)
    {
        PlayerPrefs.SetInt(KEY_SHALLOW_LAST_GAME_INDEX, gameIndex);
        PlayerPrefs.Save();
    }
}