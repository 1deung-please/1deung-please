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

    private void Start()
    {
        if (scratchPanel != null) scratchPanel.SetActive(false);

        // 1. FadePanel 초기화
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        // 2. 가이드 글자 Raycast 해제
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

        // 런타임용 텍스처 생성 및 원본 복사
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

        // 시작 시 초기 위치 설정
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
        if (endingStarted || scratchPanel == null || !scratchPanel.activeSelf) return;

        // 터치/마우스 누름 시작
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            HideGuideText();

            UpdateCoinPosition();
            ScratchAtMouse();
        }

        // 터치/마우스 드래그 중
        if (Input.GetMouseButton(0) && isDragging)
        {
            HideGuideText();
            UpdateCoinPosition();
            ScratchAtMouse();
        }

        // 터치/마우스 뗌 (되돌아가지 않고 현재 위치에 가만히 둠)
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // 동전 UI 위치를 터치/마우스 좌표로 이동
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

    // 초기 실행 시 혹은 복권 창을 열 때 설정된 지정 위치로 동전 배치
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
        if (scratchGuideText != null && scratchGuideText.activeSelf)
        {
            scratchGuideText.SetActive(false);
        }
    }

    private void ScratchAtMouse()
    {
        if (runtimeTexture == null || scratchGrayRect == null) return;

        Camera cam = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? parentCanvas.worldCamera : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(scratchGrayRect, Input.mousePosition, cam, out Vector2 localPosition))
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
        if (scratchPanel == null) return;

        scratchPanel.SetActive(true);

        if (scratchGuideText != null)
            scratchGuideText.SetActive(true);

        if (scratchAfter != null)
            scratchAfter.SetActive(true);

        if (scratchGray != null)
            scratchGray.SetActive(true);

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        ResetCoinPosition();
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