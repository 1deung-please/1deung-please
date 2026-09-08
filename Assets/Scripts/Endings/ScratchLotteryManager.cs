using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchLotteryManager : MonoBehaviour
{
    [Header("Lottery UI")]
    [SerializeField] private GameObject scratchPanel;
    [SerializeField] private GameObject scratchBefore;
    [SerializeField] private GameObject scratchAfter;
    [SerializeField] private GameObject scratchGuideText;

    [Header("Cursor")]
    [SerializeField] private Texture2D coinCursor;

    [Header("Scratch Settings")]
    [SerializeField] private int brushSize = 50;
    [SerializeField] private float requiredPercent = 60f; // 목표 긁기 비율 (60%)

    [Header("Ending Fade")]
    [SerializeField] private CanvasGroup fadePanel; // 흰색 이미지 연동 CanvasGroup
    [SerializeField] private float fadeDuration = 1.0f; // 흰색으로 변하는 시간 (초 단위)

    [Header("Ticket Area Normalized (0~1)")]
    [SerializeField] private Rect ticketAreaNormalized = new Rect(0.28f, 0.20f, 0.44f, 0.25f);

    private Texture2D runtimeTexture;
    private RectTransform scratchRect;

    private RawImage beforeRawImage;
    private Image beforeImage;

    private bool[] scratchablePixels;
    private bool[] erasedPixels;

    private int scratchablePixelCount;
    private int erasedPixelCount;

    private bool isDragging = false;
    private bool endingStarted = false;

    private void Start()
    {
        if (scratchPanel != null) scratchPanel.SetActive(false);

        // 1. FadePanel 초기화 (투명 상태)
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

        if (scratchBefore == null || scratchAfter == null)
        {
            Debug.LogError("ScratchBefore 또는 ScratchAfter가 연결되지 않았습니다!");
            return;
        }

        beforeRawImage = scratchBefore.GetComponent<RawImage>();
        beforeImage = scratchBefore.GetComponent<Image>();

        Texture2D original = GetOriginalTexture();
        if (original == null || !original.isReadable)
        {
            Debug.LogError("Texture2D를 읽을 수 없거나 Read/Write Enabled가 꺼져 있습니다.");
            return;
        }

        scratchRect = scratchBefore.GetComponent<RectTransform>();

        runtimeTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        Color[] sourcePixels = original.GetPixels();

        scratchablePixels = new bool[sourcePixels.Length];
        erasedPixels = new bool[sourcePixels.Length];

        scratchablePixelCount = 0;
        erasedPixelCount = 0;

        int width = original.width;
        int height = original.height;

        int minX = Mathf.FloorToInt(ticketAreaNormalized.xMin * width);
        int maxX = Mathf.CeilToInt(ticketAreaNormalized.xMax * width);
        int minY = Mathf.FloorToInt(ticketAreaNormalized.yMin * height);
        int maxY = Mathf.CeilToInt(ticketAreaNormalized.yMax * height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = y * width + x;
                Color pixel = sourcePixels[i];

                if (pixel.a <= 0.1f) continue;

                bool isInTicketArea = (x >= minX && x <= maxX && y >= minY && y <= maxY);
                if (!isInTicketArea) continue;

                Color.RGBToHSV(pixel, out float h, out float s, out float v);

                bool isPureGray = (s < 0.15f) && (v > 0.2f && v < 0.85f);

                if (isPureGray)
                {
                    scratchablePixels[i] = true;
                    scratchablePixelCount++;
                }
            }
        }

        runtimeTexture.SetPixels(sourcePixels);
        runtimeTexture.Apply();

        ApplyRuntimeTexture();
        scratchAfter.SetActive(true);
    }

    private Texture2D GetOriginalTexture()
    {
        if (beforeRawImage != null && beforeRawImage.texture != null)
            return beforeRawImage.texture as Texture2D;

        if (beforeImage != null && beforeImage.sprite != null)
            return beforeImage.sprite.texture;

        return null;
    }

    private void ApplyRuntimeTexture()
    {
        if (beforeRawImage != null)
        {
            beforeRawImage.texture = runtimeTexture;
        }
        else if (beforeImage != null)
        {
            beforeImage.sprite = Sprite.Create(
                runtimeTexture,
                new Rect(0, 0, runtimeTexture.width, runtimeTexture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    private void Update()
    {
        if (endingStarted || scratchPanel == null || !scratchPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            HideGuideText();
            ScratchAtMouse();
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            HideGuideText();
            ScratchAtMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
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
        if (runtimeTexture == null || scratchRect == null) return;

        Canvas canvas = scratchBefore.GetComponentInParent<Canvas>();
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(scratchRect, Input.mousePosition, cam, out Vector2 localPosition))
        {
            float width = scratchRect.rect.width;
            float height = scratchRect.rect.height;

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

        // 60% 이상 긁혔을 때 연출 시작
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

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        if (coinCursor != null)
        {
            Cursor.SetCursor(coinCursor, new Vector2(coinCursor.width / 2f, coinCursor.height / 2f), CursorMode.Auto);
        }
    }

    private void StartEnding()
    {
        if (endingStarted) return;
        endingStarted = true;

        StartCoroutine(FadeToWhiteAndChangeScene());
    }

    // 60% 달성 시 점차 흰색으로 변한 뒤 씬을 전환하는 코루틴
    private IEnumerator FadeToWhiteAndChangeScene()
    {
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true; // 연출 중 조작 방지

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Alpha를 0에서 1로 천천히 변경
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        // 커서 원복
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // 다음 씬 전환
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