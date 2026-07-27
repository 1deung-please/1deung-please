using UnityEngine;
using System.Collections;
using TMPro;

public enum MiniGameKind { PickTrash, DontMove, LogicFortress }

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("Popup UI")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    public float popupDuration = 2f;

    [Header("Achievement Data")]
    public AchievementListData achievementList; // 20개 이름/뱃지 정보

    private Coroutine popupCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnTutorialNoButtonClicked()
    {
        var data = GameManager.Instance.gameData;
        data.tutorialNoButtonCount++;
        if (data.tutorialNoButtonCount >= 10)
            TryUnlock(1);
    }

    public void OnMiniGameResult(MiniGameKind kind, bool success)
    {
        var data = GameManager.Instance.gameData;
        int idx = (int)kind;

        if (success)
        {
            data.consecutiveSuccess[idx]++;
            data.consecutiveFail[idx] = 0;

            if (data.consecutiveSuccess[idx] >= 10)
            {
                if (kind == MiniGameKind.PickTrash) TryUnlock(9);
                if (kind == MiniGameKind.DontMove) TryUnlock(10);
                if (kind == MiniGameKind.LogicFortress) TryUnlock(11);
            }
        }
        else
        {
            data.consecutiveFail[idx]++;
            data.consecutiveSuccess[idx] = 0;

            if (data.consecutiveFail[idx] >= 5)
            {
                if (kind == MiniGameKind.DontMove) TryUnlock(2);
                if (kind == MiniGameKind.PickTrash) TryUnlock(3);
                if (kind == MiniGameKind.LogicFortress) TryUnlock(4);
            }
        }
    }

    public void OnGlobalTimerEnd()
    {
        bool[] played = GameManager.Instance.gameData.playedGames;

        bool none = !played[0] && !played[1] && !played[2];
        bool onlyTrash = played[0] && !played[1] && !played[2];
        bool onlyLogic = played[2] && !played[0] && !played[1];
        bool onlyDontMove = played[1] && !played[0] && !played[2];

        if (none) TryUnlock(5);
        if (onlyTrash) TryUnlock(6);
        if (onlyLogic) TryUnlock(7);
        if (onlyDontMove) TryUnlock(8);
    }

    public void OnEndingConfirmed(string endingId)
    {
        switch (endingId)
        {
            case "얄팍한속셈": TryUnlock(14); break;
            case "자격미달": TryUnlock(15); break;
            case "절반의성공": TryUnlock(16); break;
            case "진정한귀인": TryUnlock(17); break;
            case "히든": TryUnlock(18); break;
        }

        TryUnlock(13);

        if (AchievementStorage.IsUnlocked(14) && AchievementStorage.IsUnlocked(15)
            && AchievementStorage.IsUnlocked(16) && AchievementStorage.IsUnlocked(17))
        {
            TryUnlock(12);
        }
    }

    void TryUnlock(int id)
    {
        if (AchievementStorage.IsUnlocked(id)) return;
        AchievementStorage.Unlock(id);
        ShowUnlockPopup(id);
    }

    void ShowUnlockPopup(int id)
    {
        if (popupPanel == null || popupText == null) return;

        string title = GetAchievementTitle(id);
        popupText.text = $"업적 달성: {title}";
        popupPanel.SetActive(true);

        if (popupCoroutine != null) StopCoroutine(popupCoroutine);
        popupCoroutine = StartCoroutine(HidePopupAfterDelay());
    }

    string GetAchievementTitle(int id)
    {
        if (achievementList == null) return $"업적 {id}";

        foreach (var info in achievementList.achievements)
        {
            if (info.id == id) return info.title;
        }
        return $"업적 {id}";
    }

    IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        popupPanel.SetActive(false);
    }
}