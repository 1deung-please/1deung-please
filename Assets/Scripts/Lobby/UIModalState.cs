// 현재 열려있는 모달(팝업/패널) 개수를 전역으로 추적.
// 여러 팝업이 동시에 열릴 수 있어서 bool 하나가 아니라 카운트로 관리.
public static class UIModalState
{
    private static int openCount = 0;

    public static bool IsAnyModalOpen => openCount > 0;

    public static void Register()
    {
        openCount++;
    }

    public static void Unregister()
    {
        if (openCount > 0) openCount--;
    }
}