using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResumePopupManager : MonoBehaviour
{
    public static ResumePopupManager Instance;

    [SerializeField] private GameObject resumePopup;

    [Header("닫기")]
    [Tooltip("X 버튼 위에 얹은 투명 Button들. OnClick을 따로 연결하지 않아도 눌리면 팝업이 닫힙니다. 비워두면 팝업 안의 모든 Button을 닫기 버튼으로 씁니다.")]
    [SerializeField] private Button[] closeButtons;

    // 종료 팝업(32000)보다는 아래(종료 팝업이 이 팝업 위에도 떠야 함), 그 외 모든 캔버스보다는 위
    private const int PopupSortingOrder = 30000;

    public bool IsPopupActive => resumePopup != null && resumePopup.activeSelf;

    // 팝업이 떠 있는 동안 클릭 판정을 꺼둔 다른 캔버스들의 GraphicRaycaster 목록
    private readonly List<GraphicRaycaster> suspendedRaycasters = new List<GraphicRaycaster>();

    private bool isOpen = false;
    private float timeScaleBeforePopup = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AutoFindReferences();
            ConfigureCanvas();
            WireCloseButtons();

            if (resumePopup != null)
                resumePopup.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 닫기 버튼이 아닌 다른 경로로 팝업이 꺼졌더라도 일시정지가 풀리지 않은 채 남지 않게 한다
        if (isOpen && !IsPopupActive)
            Close();
    }

    private void OnDestroy()
    {
        RestoreRaycasters();
    }

    // 팝업 표시: 전역 타이머 + 게임 시간 정지
    public void Show()
    {
        if (resumePopup == null)
        {
            // 팝업이 연결되어 있지 않으면 예전처럼 바로 이어서 시작
            if (GameManager.Instance != null)
                GameManager.Instance.ResumeTimer();
            return;
        }

        if (isOpen) return;
        isOpen = true;

        timeScaleBeforePopup = Time.timeScale;

        if (GameManager.Instance != null)
            GameManager.Instance.PauseTimer();

        Time.timeScale = 0f;

        resumePopup.transform.SetAsLastSibling();
        resumePopup.SetActive(true);

        SuspendOtherRaycasters();
    }

    // 팝업을 닫고 열기 전 상태로 되돌려 이어서 시작 (X 영역 클릭 시 호출됨)
    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        if (resumePopup != null)
            resumePopup.SetActive(false);

        RestoreRaycasters();

        Time.timeScale = timeScaleBeforePopup;

        if (GameManager.Instance != null)
            GameManager.Instance.ResumeTimer();
    }

    // 인스펙터에서 연결하지 않았을 때를 위한 자동 탐색
    private void AutoFindReferences()
    {
        if (resumePopup == null) return;

        // 닫기 버튼 목록이 비어 있으면 팝업 안의 모든 Button을 닫기 버튼으로 쓴다
        if (closeButtons == null || closeButtons.Length == 0)
            closeButtons = resumePopup.GetComponentsInChildren<Button>(true);
    }

    // closeButtons의 클릭을 Close()에 연결 (OnClick을 인스펙터에서 따로 연결할 필요 없음)
    private void WireCloseButtons()
    {
        if (closeButtons == null) return;

        foreach (Button button in closeButtons)
        {
            if (button != null)
                button.onClick.AddListener(Close);
        }
    }

    // 다른 어떤 캔버스보다 위에 보이고 클릭도 먼저 받도록 Overlay + 높은 정렬 순서로 맞춘다
    private void ConfigureCanvas()
    {
        if (resumePopup == null) return;

        Canvas popupCanvas = resumePopup.GetComponentInParent<Canvas>(true);
        if (popupCanvas == null) return;

        Canvas rootCanvas = popupCanvas.rootCanvas;
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = PopupSortingOrder;

        if (popupCanvas != rootCanvas && popupCanvas.overrideSorting)
            popupCanvas.sortingOrder = PopupSortingOrder;
    }

    // 팝업이 떠 있는 동안에는 팝업 자신의 캔버스를 뺀 모든 캔버스의 클릭 판정을 끈다.
    private void SuspendOtherRaycasters()
    {
        if (resumePopup == null) return;

        suspendedRaycasters.Clear();

        Canvas popupCanvas = resumePopup.GetComponentInParent<Canvas>();
        Canvas popupRoot = popupCanvas != null ? popupCanvas.rootCanvas : null;

        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            if (!raycaster.enabled) continue;

            Canvas canvas = raycaster.GetComponent<Canvas>();
            if (canvas != null && popupRoot != null && canvas.rootCanvas == popupRoot)
                continue;

            // 종료 팝업(Esc)은 이 팝업 위에서도 눌려야 하므로 건드리지 않는다
            if (ExitPopupManager.Instance != null &&
                raycaster.transform.IsChildOf(ExitPopupManager.Instance.transform))
                continue;

            raycaster.enabled = false;
            suspendedRaycasters.Add(raycaster);
        }
    }

    private void RestoreRaycasters()
    {
        foreach (GraphicRaycaster raycaster in suspendedRaycasters)
        {
            if (raycaster != null)
                raycaster.enabled = true;
        }

        suspendedRaycasters.Clear();
    }
}