using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    // ---- 팝업 큐 시스템 ----
    // 엔딩 우선순위: 얄팍한속셈 > 자격미달 > 절반의성공 > 진정한귀인 > 히든
    static readonly List<string> endingPriorityOrder = new List<string>
    {
        "얄팍한속셈", "자격미달", "절반의성공", "진정한귀인", "히든"
    };

    class PopupRequest
    {
        public bool isEnding;      // true: 엔딩, false: 업적
        public string endingId;    // 엔딩일 때
        public int achievementId;  // 업적일 때 (1~20, 오름차순 우선순위)
    }

    private readonly List<PopupRequest> pendingQueue = new List<PopupRequest>();
    private bool isShowingPopup = false;
    public bool IsPopupActive => isShowingPopup;

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
            case "얄팍한속셈": TryUnlock(14); EnqueueEnding(endingId); break;
            case "자격미달": TryUnlock(15); EnqueueEnding(endingId); break;
            case "절반의성공": TryUnlock(16); EnqueueEnding(endingId); break;
            case "진정한귀인": TryUnlock(17); EnqueueEnding(endingId); break;
            case "히든": TryUnlock(18); EnqueueEnding(endingId); break;
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
        EnqueueAchievement(id);
    }

    // ---- 큐에 추가 + 정렬 + 처리 시작 ----

    void EnqueueAchievement(int id)
    {
        pendingQueue.Add(new PopupRequest { isEnding = false, achievementId = id });
        SortQueue();
        TryProcessNext();
    }

    void EnqueueEnding(string endingId)
    {
        pendingQueue.Add(new PopupRequest { isEnding = true, endingId = endingId });
        SortQueue();
        TryProcessNext();
    }

    // 엔딩 > 업적, 엔딩끼리는 지정된 순서, 업적끼리는 id 오름차순
    void SortQueue()
    {
        pendingQueue.Sort((a, b) =>
        {
            if (a.isEnding != b.isEnding)
                return a.isEnding ? -1 : 1; // 엔딩이 항상 먼저

            if (a.isEnding && b.isEnding)
            {
                int ai = endingPriorityOrder.IndexOf(a.endingId);
                int bi = endingPriorityOrder.IndexOf(b.endingId);
                if (ai < 0) ai = int.MaxValue;
                if (bi < 0) bi = int.MaxValue;
                return ai.CompareTo(bi);
            }

            // 둘 다 업적
            return a.achievementId.CompareTo(b.achievementId);
        });
    }

    void TryProcessNext()
    {
        if (isShowingPopup) return;
        if (pendingQueue.Count == 0) return;

        PopupRequest next = pendingQueue[0];
        pendingQueue.RemoveAt(0);

        isShowingPopup = true;

        if (next.isEnding)
            ShowEndingPopup(next.endingId);
        else
            ShowAchievementPopup(next.achievementId);
    }

    void ShowAchievementPopup(int id)
    {
        if (achievementPopup == null)
        {
            OnPopupFinished();
            return;
        }

        var info = GetAchievementInfo(id);
        if (info == null)
        {
            OnPopupFinished();
            return;
        }

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

        // 자동 닫힘 없이, 화면 터치(팝업 버튼/오버레이 클릭)로만 닫힘 - CloseAchievementPopup() 참고
    }

    static readonly Dictionary<string, string> endingColors = new Dictionary<string, string>
    {
        { "얄팍한속셈", "#8B7C47" },
        { "자격미달", "#616F7C" },
        { "절반의성공", "#83C1DB" },
        { "진정한귀인", "#FEFCA1" },
        { "히든", "#9187B9" },
    };

    void ShowEndingPopup(string endingId)
    {
        if (endingPopup == null)
        {
            OnPopupFinished();
            return;
        }

        var info = GetEndingInfo(endingId);

        if (endingBadgeImage != null && info != null && info.unlockedIcon != null)
        {
            endingBadgeImage.sprite = info.unlockedIcon;
            endingBadgeImage.rectTransform.anchoredPosition = info.imageOffset;
        }

        string color = endingColors.ContainsKey(endingId) ? endingColors[endingId] : "#FFFFFF";
        string title = info != null ? info.title : endingId;

        if (endingTitleText != null)
            endingTitleText.text = $"<color={color}>{title}</color>";

        if (endingBodyText != null)
            endingBodyText.text = "엔딩을 획득하였습니다.";

        if (sfxSource != null && endingSfx != null)
            sfxSource.PlayOneShot(endingSfx);

        endingPopup.SetActive(true);

        // 자동 닫힘 없이, 화면 터치(팝업 버튼/오버레이 클릭)로만 닫힘 - CloseEndingPopup() 참고
    }

    void OnPopupFinished()
    {
        isShowingPopup = false;
        TryProcessNext();
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

    // 외부(버튼 등)에서 즉시 닫고 싶을 때 - 큐 처리도 이어서 진행
    public void CloseAchievementPopup()
    {
        StopAllCoroutines();
        if (achievementPopup != null)
            achievementPopup.SetActive(false);
        OnPopupFinished();
    }

    public void CloseEndingPopup()
    {
        StopAllCoroutines();
        if (endingPopup != null)
            endingPopup.SetActive(false);
        OnPopupFinished();
    }

    // ==========================================
    // [테스트용] 팝업 강제 노출 메서드 (큐 우회, 즉시 표시)
    // ==========================================

    public void ForceShowAchievementPopup(int id)
    {
        Debug.Log($"[AchievementManager] 업적 팝업 강제 노출 테스트 (ID: {id})");
        ShowAchievementPopup(id);
    }

    public void ForceShowEndingPopup(string endingId)
    {
        Debug.Log($"[AchievementManager] 엔딩 팝업 강제 노출 테스트 (ID: {endingId})");
        ShowEndingPopup(endingId);
    }

    [ContextMenu("Test - Show Popup (ID: 1)")]
    private void TestShowPopupID1()
    {
        ForceShowAchievementPopup(1);
    }

    [ContextMenu("Test - Show Ending Popup (진정한귀인)")]
    private void TestShowEndingPopupTrueBenefactor()
    {
        ForceShowEndingPopup("진정한귀인");
    }

    [ContextMenu("Reset Only Achievements Data")]
    public void ResetOnlyAchievementsData()
    {
        AchievementStorage.ClearAllAchievements();
    }
}