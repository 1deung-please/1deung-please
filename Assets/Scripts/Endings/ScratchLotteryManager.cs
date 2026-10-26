using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchLotteryManager : MonoBehaviour
{
    [Header("Lottery UI")]
    [SerializeField] private GameObject scratchPanel;
    [SerializeField] private GameObject scratchBefore;
    [SerializeField] private GameObject scratchGray;
    [SerializeField] private GameObject scratchAfter;
    [SerializeField] private GameObject scratchGuideText;

    [Header("Coin UI Settings")]
    [SerializeField] private RectTransform coinUI;
    [SerializeField] private Vector2 coinDefaultPosition = new Vector2(281f, 57f);
    [SerializeField] private Vector2 coinOffset = new Vector2(0f, 50f);

    [Header("Scratch Settings")]
    [SerializeField] private int brushSize = 50;
    [SerializeField] private float requiredPercent = 60f;

    [Header("Ending Fade")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1.0f;

    private Texture2D runtimeTexture;
    private RectTransform scratchGrayRect;

    private RawImage grayRawImage;
    private Image grayImage;

    private bool[] scratchablePixels;
    private bool[] erasedPixels;

    private int scratchablePixelCount;
    private int erasedPixelCount;

    private bool isDragging = false;
    private bool endingStarted = false;
    private Canvas parentCanvas;

    private Coroutine guideTextCoroutine;
    private bool wasGuideTextVisible = false;
    private Vector2? lastMousePosition = null;

    private void Start()
    {
        if (scratchPanel != null) scratchPanel.SetActive(false);

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        if (scratchGuideText != null)
        {
            Graphic guideGraphic = scratchGuideText.GetComponent<Graphic>();
            if (guideGraphic != null) guideGraphic.raycastTarget = false;
        }

        if (scratchGray == null || scratchAfter == null)
        {
            Debug.LogError("ScratchGray 또는 ScratchAfter가 연결되지 않았습니다!");
            return;
        }

        parentCanvas = scratchGray.GetComponentInParent<Canvas>();
        grayRawImage = scratchGray.GetComponent<RawImage>();
        grayImage = scratchGray.GetComponent<Image>();

        Texture2D original = GetOriginalTexture();
        if (original == null || !original.isReadable)
        {
            Debug.LogError("scratchGray의 Texture2D를 읽을 수 없거나 Read/Write Enabled가 꺼져 있습니다.");
            return;
        }

        scratchGrayRect = scratchGray.GetComponent<RectTransform>();

        runtimeTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        Color[] sourcePixels = original.GetPixels();

        scratchablePixels = new bool[sourcePixels.Length];
        erasedPixels = new bool[sourcePixels.Length];

        scratchablePixelCount = 0;
        erasedPixelCount = 0;

        for (int i = 0; i < sourcePixels.Length; i++)
        {
            if (sourcePixels[i].a > 0.1f)
            {
                scratchablePixels[i] = true;
                scratchablePixelCount++;
            }
        }

        runtimeTexture.SetPixels(sourcePixels);
        runtimeTexture.Apply();

        ApplyRuntimeTexture();
        scratchAfter.SetActive(true);

        ResetCoinPosition();
    }

    private Texture2D GetOriginalTexture()
    {
        if (grayRawImage != null && grayRawImage.texture != null)
            return grayRawImage.texture as Texture2D;

        if (grayImage != null && grayImage.sprite != null)
            return grayImage.sprite.texture;

        return null;
    }

    private void ApplyRuntimeTexture()
    {
        if (grayRawImage != null)
        {
            grayRawImage.texture = runtimeTexture;
        }
        else if (grayImage != null)
        {
            grayImage.sprite = Sprite.Create(
                runtimeTexture,
                new Rect(0, 0, runtimeTexture.width, runtimeTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    private void Update()
    {
        CheckGuideTextVisibility();

        if (endingStarted || scratchPanel == null || !scratchPanel.activeSelf) return;

        // 종료 팝업/업적·엔딩 팝업이 떠 있는 동안에는 터치를 긁기/동전 이동/안내 문구 숨김으로 처리하지 않는다.
        // 드래그 상태와 마지막 위치를 비워서, 팝업을 닫은 뒤 처음 긁을 때
        // 팝업 열기 전 위치에서 현재 위치까지 한 번에 긁히지 않게 한다.
        if (UIInputGate.IsBlocked)
        {
            isDragging = false;
            lastMousePosition = null;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            HideGuideText();

            UpdateCoinPosition();
            ScratchAtMouse(Input.mousePosition);
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            HideGuideText();
            UpdateCoinPosition();

            if (lastMousePosition.HasValue)
            {
                float distance = Vector2.Distance(lastMousePosition.Value, Input.mousePosition);
                float step = Mathf.Max(1f, brushSize * 0.25f);
                int steps = Mathf.CeilToInt(distance / step);

                for (int i = 0; i <= steps; i++)
                {
                    float t = (steps == 0) ? 0f : (float)i / steps;
                    Vector2 samplePoint = Vector2.Lerp(lastMousePosition.Value, Input.mousePosition, t);
                    ScratchAtMouse(samplePoint);
                }
            }
            else
            {
                ScratchAtMouse(Input.mousePosition);
            }

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            lastMousePosition = null;
        }
    }

    // scratchGuideText가 (부모 포함해서) 실제로 화면에 보이기 시작하는 순간을 감지해
    // 그 순간부터 1.5초 자동 숨김 타이머를 시작한다.
    private void CheckGuideTextVisibility()
    {
        if (scratchGuideText == null) return;

        bool isVisibleNow = scratchGuideText.activeInHierarchy;

        if (isVisibleNow && !wasGuideTextVisible)
        {
            Debug.Log("[Lottery] 가이드텍스트가 실제로 화면에 보이기 시작함 -> 자동 숨김 타이머 시작");
            if (guideTextCoroutine != null) StopCoroutine(guideTextCoroutine);
            guideTextCoroutine = StartCoroutine(HideGuideTextAfterDelay(1.5f));
        }
        else if (!isVisibleNow && guideTextCoroutine != null)
        {
            // 보이지 않게 된 상태(예: 패널이 다시 꺼짐)라면 진행 중이던 타이머는 정리
            StopCoroutine(guideTextCoroutine);
            guideTextCoroutine = null;
        }

        wasGuideTextVisible = isVisibleNow;
    }

    private void UpdateCoinPosition()
    {
        if (coinUI == null || parentCanvas == null) return;

        Camera cam = (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? parentCanvas.worldCamera : null;

        RectTransform coinParentRect = coinUI.parent as RectTransform;
        if (coinParentRect != null)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(coinParentRect, Input.mousePosition, cam, out Vector2 localPoint))
            {
                coinUI.anchoredPosition = localPoint + coinOffset;
            }
        }
    }

    private void ResetCoinPosition()
    {
        if (coinUI != null)
        {
            coinUI.gameObject.SetActive(true);
            coinUI.anchoredPosition = coinDefaultPosition;
        }
    }

    public void HideGuideText()
    {
        if (guideTextCoroutine != null)
        {
            StopCoroutine(guideTextCoroutine);
            guideTextCoroutine = null;
        }

        if (scratchGuideText != null && scratchGuideText.activeSelf)
        {
            scratchGuideText.SetActive(false);
        }
    }

    private void ScratchAtMouse(Vector2 screenPos)
    {
        if (runtimeTexture == null || scratchGrayRect == null) return;

        Camera cam = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? parentCanvas.worldCamera : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(scratchGrayRect, screenPos, cam, out Vector2 localPosition))
        {
            float width = scratchGrayRect.rect.width;
            float height = scratchGrayRect.rect.height;

            float normalizedX = (localPosition.x + width * 0.5f) / width;
            float normalizedY = (localPosition.y + height * 0.5f) / height;

            if (normalizedX < 0f || normalizedX > 1f || normalizedY < 0f || normalizedY > 1f) return;

            int pixelX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * runtimeTexture.width), 0, runtimeTexture.width - 1);
            int pixelY = Mathf.Clamp(Mathf.FloorToInt(normalizedY * runtimeTexture.height), 0, runtimeTexture.height - 1);

            EraseCircle(pixelX, pixelY);
            runtimeTexture.Apply();

            CheckScratchPercent();
        }
    }

    private void EraseCircle(int centerX, int centerY)
    {
        int radius = brushSize / 2;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radius * radius) continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= runtimeTexture.width || py < 0 || py >= runtimeTexture.height) continue;

                int index = py * runtimeTexture.width + px;

                if (!scratchablePixels[index]) continue;

                if (!erasedPixels[index])
                {
                    erasedPixels[index] = true;
                    erasedPixelCount++;
                }

                runtimeTexture.SetPixel(px, py, Color.clear);
            }
        }
    }

    private void CheckScratchPercent()
    {
        if (scratchablePixelCount <= 0) return;

        float percent = ((float)erasedPixelCount / scratchablePixelCount) * 100f;
        Debug.Log("복권 긁은 정도 : " + percent.ToString("F1") + "%");

        if (percent >= requiredPercent)
        {
            StartEnding();
        }
    }

    public void ShowLottery()
    {
        Debug.Log($"[Lottery] ShowLottery 호출됨. 시각={Time.realtimeSinceStartup:F2}, 활성상태={gameObject.activeInHierarchy}");

        if (scratchPanel == null) return;

        scratchPanel.SetActive(true);

        if (scratchGuideText != null)
        {
            scratchGuideText.SetActive(true);
            // 자동 숨김 타이머는 Update()의 CheckGuideTextVisibility()가 처리함
        }

        if (scratchAfter != null)
            scratchAfter.SetActive(true);

        if (scratchGray != null)
            scratchGray.SetActive(true);

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        ResetCoinPosition();
    }

    private IEnumerator HideGuideTextAfterDelay(float delay)
    {
        float startTime = Time.realtimeSinceStartup;
        // Time.timeScale이 0이어도 멈추지 않도록 실시간 기준으로 대기
        yield return new WaitForSecondsRealtime(delay);

        Debug.Log($"[Lottery] {delay}초 대기 완료 (실제 경과: {Time.realtimeSinceStartup - startTime:F2}초). 가이드텍스트를 끕니다.");

        if (scratchGuideText != null)
        {
            scratchGuideText.SetActive(false);
        }
        guideTextCoroutine = null;
    }

    private void StartEnding()
    {
        if (endingStarted) return;
        endingStarted = true;

        if (coinUI != null) coinUI.gameObject.SetActive(false);

        StartCoroutine(FadeToWhiteAndChangeScene());
    }

    private IEnumerator FadeToWhiteAndChangeScene()
    {
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("Ending_Common");
        }
        else
        {
            Debug.LogError("SceneLoader.Instance가 존재하지 않습니다.");
        }
    }
}