using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public enum MiniGameKind { PickTrash, DontMove, LogicFortress }

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("업적 팝업")]
    public GameObject achievementPopup;
    public Image achievementBadgeImage;
    public TMP_Text achievementTitleText;
    public TMP_Text achievementDescriptionText;
    public TMP_Text achievementBodyText;
    public float popupDuration = 3f;

    [Header("엔딩 팝업")]
    public GameObject endingPopup;
    public Image endingBadgeImage;
    public TMP_Text endingTitleText;
    public TMP_Text endingBodyText;

    [Header("효과음")]
    public AudioSource sfxSource;
    public AudioClip achievementSfx;
    public AudioClip endingSfx;

    [Header("Achievement Data")]
    public AchievementListData achievementList;
    public EndingListData endingList;

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
            case "얄팍한속셈": TryUnlock(14); ShowEndingPopup(endingId); break;
            case "자격미달": TryUnlock(15); ShowEndingPopup(endingId); break;
            case "절반의성공": TryUnlock(16); ShowEndingPopup(endingId); break;
            case "진정한귀인": TryUnlock(17); ShowEndingPopup(endingId); break;
            case "히든": TryUnlock(18); ShowEndingPopup(endingId); break;
        }

        TryUnlock(13);

        if (AchievementStorage.IsUnlocked(14) && AchievementStorage.IsUnlocked(15)
            && AchievementStorage.IsUnlocked(16) && AchievementStorage.IsUnlocked(17))
        {
            TryUnlock(12);
        }
    }

    public void TryUnlockPublic(int id) => TryUnlock(id);

    void TryUnlock(int id)
    {
        if (AchievementStorage.IsUnlocked(id)) return;
        AchievementStorage.Unlock(id);
        ShowAchievementPopup(id);
    }

    void ShowAchievementPopup(int id)
    {
        if (achievementPopup == null) return;

        var info = GetAchievementInfo(id);
        if (info == null) return;

        if (achievementBadgeImage != null && info.badge != null)
            achievementBadgeImage.sprite = info.badge;

        if (achievementTitleText != null)
            achievementTitleText.text = info.title;

        if (achievementDescriptionText != null)
            achievementDescriptionText.text = info.description;

        if (achievementBodyText != null)
            achievementBodyText.text = "업적을 달성하였습니다.";

        if (sfxSource != null && achievementSfx != null)
            sfxSource.PlayOneShot(achievementSfx);

        achievementPopup.SetActive(true);

        if (popupCoroutine != null) StopCoroutine(popupCoroutine);
        popupCoroutine = StartCoroutine(HideAchievementAfterDelay());
    }

    static readonly System.Collections.Generic.Dictionary<string, string> endingColors = new System.Collections.Generic.Dictionary<string, string>
    {
        { "얄팍한속셈", "#8B7C47" },
        { "자격미달", "#616F7C" },
        { "절반의성공", "#83C1DB" },
        { "진정한귀인", "#FEFCA1" },
        { "히든", "#9187B9" },
    };

    void ShowEndingPopup(string endingId)
    {
        if (endingPopup == null) return;

        var info = GetEndingInfo(endingId);

        if (endingBadgeImage != null && info != null && info.unlockedIcon != null)
            endingBadgeImage.sprite = info.unlockedIcon;

        string color = endingColors.ContainsKey(endingId) ? endingColors[endingId] : "#FFFFFF";
        string title = info != null ? info.title : endingId;

        if (endingTitleText != null)
            endingTitleText.text = $"<color={color}>{title}</color>";

        if (endingBodyText != null)
            endingBodyText.text = "엔딩을 획득하였습니다.";

        if (sfxSource != null && endingSfx != null)
            sfxSource.PlayOneShot(endingSfx);

        endingPopup.SetActive(true);
    }

    AchievementInfo GetAchievementInfo(int id)
    {
        if (achievementList == null) return null;
        foreach (var info in achievementList.achievements)
            if (info.id == id) return info;
        return null;
    }

    EndingInfo GetEndingInfo(string endingId)
    {
        if (endingList == null) return null;
        foreach (var info in endingList.endings)
            if (info.endingId == endingId) return info;
        return null;
    }

    IEnumerator HideAchievementAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        if (achievementPopup != null)
            achievementPopup.SetActive(false);
    }
}