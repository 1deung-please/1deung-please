using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitPopupManager : MonoBehaviour
{
    public static ExitPopupManager Instance;

    [SerializeField] private GameObject exitConfirmPopup;

    public bool IsPopupActive => exitConfirmPopup != null && exitConfirmPopup.activeSelf;

    private const int TopSortingOrder = 32000;

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
        if (suspendedRaycasters.Count > 0 && !IsPopupActive)
            RestoreOtherRaycasters();
    }

    private void OnDestroy()
    {
        RestoreOtherRaycasters();
        AudioListener.pause = false;
    }

    public void ShowPopup()
    {
        if (exitConfirmPopup != null && !exitConfirmPopup.activeSelf)
        {
            exitConfirmPopup.transform.SetAsLastSibling();

            exitConfirmPopup.SetActive(true);
            SuspendOtherRaycasters();

            AudioListener.pause = true;
        }
    }

    public void HidePopup()
    {
        if (exitConfirmPopup != null)
        {
            exitConfirmPopup.SetActive(false);
        }

        RestoreOtherRaycasters();

        AudioListener.pause = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeTimerAfterExitPopup();
        }
    }

    public void ConfirmExit()
    {
        AudioListener.pause = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGameData();
        }

        Application.Quit();
    }

    private void EnsurePopupOnTop()
    {
        if (exitConfirmPopup == null) return;

        Canvas popupCanvas = exitConfirmPopup.GetComponentInParent<Canvas>(true);
        if (popupCanvas == null) return;

        Canvas rootCanvas = popupCanvas.rootCanvas;
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = TopSortingOrder;

        if (popupCanvas != rootCanvas && popupCanvas.overrideSorting)
            popupCanvas.sortingOrder = TopSortingOrder;
    }

    private void SuspendOtherRaycasters()
    {
        if (exitConfirmPopup == null) return;

        suspendedRaycasters.Clear();

        Canvas popupCanvas = exitConfirmPopup.GetComponentInParent<Canvas>();
        Canvas popupRoot = popupCanvas != null ? popupCanvas.rootCanvas : null;

        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            if (!raycaster.enabled) continue;

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