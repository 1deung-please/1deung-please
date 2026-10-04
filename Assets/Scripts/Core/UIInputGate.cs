using UnityEngine;

// Input.GetMouseButtonDown 등으로 화면 터치를 직접 읽는 스크립트들이 공통으로 쓰는 "입력 차단" 판정.
// 종료 팝업이나 업적/엔딩 팝업이 떠 있는 동안, 그 팝업을 누르는 터치가
// 뒤쪽 대화창/게임 진행으로 새어 들어가지 않게 한다.
//
// 사용법: 터치를 읽기 전에
//     if (UIInputGate.IsBlocked) return;
//
// - 팝업이 닫힌 "직후 한 프레임"도 차단한다. 팝업의 닫기 버튼은 손을 뗄 때(OnClick) 닫히는데,
//   아주 빠른 탭이면 같은 프레임에 팝업이 닫히고 스크립트의 Update가 그 터치를 새로 읽어버릴 수 있기 때문.
public static class UIInputGate
{
    private static int lastEvaluatedFrame = -1;
    private static bool blockedNow = false;
    private static bool blockedPrev = false;

    public static bool IsBlocked
    {
        get
        {
            // 한 프레임에 여러 스크립트가 물어봐도 판정은 프레임당 한 번만 한다
            if (lastEvaluatedFrame != Time.frameCount)
            {
                blockedPrev = blockedNow;
                blockedNow = Evaluate();
                lastEvaluatedFrame = Time.frameCount;
            }

            return blockedNow || blockedPrev;
        }
    }

    private static bool Evaluate()
    {
        if (ExitPopupManager.Instance != null && ExitPopupManager.Instance.IsPopupActive)
            return true;

        if (ResumePopupManager.Instance != null && ResumePopupManager.Instance.IsPopupActive)
            return true;

        if (AchievementManager.Instance != null && AchievementManager.Instance.HasPendingPopups)
            return true;

        return false;
    }
}