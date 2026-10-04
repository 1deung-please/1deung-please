using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitPopupManager : MonoBehaviour
{
    public static ExitPopupManager Instance;

    [SerializeField] private GameObject exitConfirmPopup;

    public bool IsPopupActive => exitConfirmPopup != null && exitConfirmPopup.activeSelf;

    // 종료 팝업 캔버스의 정렬 순서 (업적/엔딩 팝업, 로딩 화면 등 다른 어떤 캔버스보다 위)
    private const int TopSortingOrder = 32000;

    // 팝업이 떠 있는 동안 클릭 판정을 꺼둔 다른 캔버스들의 GraphicRaycaster 목록
    private readonly List<GraphicRaycaster> suspendedRaycasters = new List<GraphicRaycaster>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsurePopupOnTop();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 팝업이 HidePopup()을 거치지 않고 닫힌 경우에도 다른 캔버스의 클릭 판정을 복구한다
        if (suspendedRaycasters.Count > 0 && !IsPopupActive)
            RestoreOtherRaycasters();
    }

    private void OnDestroy()
    {
        RestoreOtherRaycasters();
    }

    public void ShowPopup()
    {
        if (exitConfirmPopup != null && !exitConfirmPopup.activeSelf)
        {
            // 업적/엔딩 팝업과 같은 캔버스를 쓰고 있어도 그 위에 그려지도록 마지막 형제로 보낸다
            exitConfirmPopup.transform.SetAsLastSibling();

            exitConfirmPopup.SetActive(true);
            SuspendOtherRaycasters();
        }
    }

    public void HidePopup()
    {
        if (exitConfirmPopup != null)
        {
            exitConfirmPopup.SetActive(false);
        }

        RestoreOtherRaycasters();

        // 종료 취소 시: 팝업을 열기 전에 타이머가 돌고 있었던 경우에만 다시 시작
        // (메인메뉴/튜토리얼/결과 화면 등 원래 멈춰 있던 곳에서는 멈춘 상태 유지)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeTimerAfterExitPopup();
        }
    }

    public void ConfirmExit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGameData();
        }

        Application.Quit();
    }

    // 종료 팝업이 업적/엔딩 팝업, 로딩 화면 등 다른 어떤 캔버스보다도 항상 위에 보이고
    // 클릭도 먼저 받도록 팝업 캔버스를 Overlay + 최상위 정렬 순서로 맞춘다.
    // (Overlay 캔버스는 카메라 캔버스보다 항상 위에 그려지고 클릭 판정 우선순위도 높다)
    private void EnsurePopupOnTop()
    {
        if (exitConfirmPopup == null) return;

        Canvas popupCanvas = exitConfirmPopup.GetComponentInParent<Canvas>(true);
        if (popupCanvas == null) return;

        Canvas rootCanvas = popupCanvas.rootCanvas;
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = TopSortingOrder;

        // 팝업이 root 캔버스 안의 별도 하위 캔버스(Override Sorting)라면 그쪽도 최상위로
        if (popupCanvas != rootCanvas && popupCanvas.overrideSorting)
            popupCanvas.sortingOrder = TopSortingOrder;
    }

    // 팝업이 떠 있는 동안에는 팝업 자신의 캔버스를 뺀 모든 캔버스의 클릭 판정을 끈다.
    // (씬의 다른 UI가 클릭 판정 순서에서 팝업보다 위에 잡혀 예/아니오가 눌리지 않는 문제 방지.
    //  예: 메인메뉴의 TitleImage/Background가 팝업보다 먼저 클릭을 받아버림)
    private void SuspendOtherRaycasters()
    {
        if (exitConfirmPopup == null) return;

        suspendedRaycasters.Clear();

        Canvas popupCanvas = exitConfirmPopup.GetComponentInParent<Canvas>();
        Canvas popupRoot = popupCanvas != null ? popupCanvas.rootCanvas : null;

        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            if (!raycaster.enabled) continue;

            // 팝업이 속한 캔버스(및 그 안의 하위 캔버스)는 그대로 둔다
            Canvas canvas = raycaster.GetComponent<Canvas>();
            if (canvas != null && popupRoot != null && canvas.rootCanvas == popupRoot)
                continue;

            raycaster.enabled = false;
            suspendedRaycasters.Add(raycaster);
        }
    }

    private void RestoreOtherRaycasters()
    {
        foreach (GraphicRaycaster raycaster in suspendedRaycasters)
        {
            if (raycaster != null)
                raycaster.enabled = true;
        }

        suspendedRaycasters.Clear();
    }
}