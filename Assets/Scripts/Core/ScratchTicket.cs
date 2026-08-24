using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScratchTicket : MonoBehaviour
{
    [Header("Scratch Images")]
    public RawImage scratchBefore;
    public RawImage scratchAfter;

    [Header("Ending")]
    public EndingManager endingManager;

    [Header("Ending Fade")]
    public ScratchButtonManager scratchButtonManager;

    [Header("Scratch Settings")]
    [Range(0.01f, 0.2f)]
    public float brushSize = 0.05f;

    [Range(0.1f, 1f)]
    public float clearPercent = 0.6f;

    [Header("Ticket Area")]
    // 복권이 있는 영역을 화면 비율로 설정
    // X: 왼쪽, Y: 아래쪽, W: 너비, H: 높이
    public Rect ticketArea = new Rect(0.30f, 0.34f, 0.40f, 0.22f);

    Texture2D scratchTexture;

    int totalScratchPixels;
    int scratchedPixels;

    bool isScratching;
    bool endingCalled;

    void Start()
    {
        if (scratchBefore == null)
        {
            Debug.LogError("ScratchBefore가 연결되지 않았습니다.");
            return;
        }

        Texture sourceTexture = scratchBefore.texture;

        if (sourceTexture == null)
        {
            Debug.LogError("ScratchBefore에 이미지가 없습니다.");
            return;
        }

        Texture2D original = sourceTexture as Texture2D;

        if (original == null)
        {
            Debug.LogError("ScratchBefore Texture가 Texture2D가 아닙니다.");
            return;
        }

        scratchTexture = new Texture2D(
            original.width,
            original.height,
            TextureFormat.RGBA32,
            false
        );

        Color[] pixels = original.GetPixels();
        scratchTexture.SetPixels(pixels);
        scratchTexture.Apply();

        scratchBefore.texture = scratchTexture;

        totalScratchPixels = CountTicketPixels();
    }

    void Update()
    {
        if (endingCalled)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isScratching = true;
        }

        if (Input.GetMouseButton(0) && isScratching)
        {
            Scratch(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isScratching = false;
        }
    }

    void Scratch(Vector2 screenPosition)
    {
        RectTransform rectTransform =
            scratchBefore.rectTransform;

        Vector2 localPoint;

        Camera uiCamera = null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            uiCamera,
            out localPoint))
        {
            float normalizedX =
                (localPoint.x + rectTransform.rect.width * 0.5f)
                / rectTransform.rect.width;

            float normalizedY =
                (localPoint.y + rectTransform.rect.height * 0.5f)
                / rectTransform.rect.height;

            // 복권 영역 밖이면 무시
            if (!ticketArea.Contains(
                new Vector2(normalizedX, normalizedY)))
                return;

            int pixelX =
                Mathf.RoundToInt(normalizedX * scratchTexture.width);

            int pixelY =
                Mathf.RoundToInt(normalizedY * scratchTexture.height);

            int radius =
                Mathf.RoundToInt(
                    brushSize * scratchTexture.width);

            EraseCircle(pixelX, pixelY, radius);

            scratchTexture.Apply();

            CheckScratchPercent();
        }
    }

    void EraseCircle(int centerX, int centerY, int radius)
    {
        int radiusSquared = radius * radius;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radiusSquared)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= scratchTexture.width ||
                    py < 0 || py >= scratchTexture.height)
                    continue;

                Color pixel = scratchTexture.GetPixel(px, py);

                if (pixel.a > 0f)
                {
                    pixel.a = 0f;
                    scratchTexture.SetPixel(px, py, pixel);

                    if (IsInsideTicket(px, py))
                    {
                        scratchedPixels++;
                    }
                }
            }
        }
    }

    bool IsInsideTicket(int x, int y)
    {
        float normalizedX =
            (float)x / scratchTexture.width;

        float normalizedY =
            (float)y / scratchTexture.height;

        return ticketArea.Contains(
            new Vector2(normalizedX, normalizedY));
    }

    int CountTicketPixels()
    {
        int count = 0;

        int minX =
            Mathf.FloorToInt(
                ticketArea.xMin * scratchTexture.width);

        int maxX =
            Mathf.CeilToInt(
                ticketArea.xMax * scratchTexture.width);

        int minY =
            Mathf.FloorToInt(
                ticketArea.yMin * scratchTexture.height);

        int maxY =
            Mathf.CeilToInt(
                ticketArea.yMax * scratchTexture.height);

        count =
            (maxX - minX) *
            (maxY - minY);

        return count;
    }

    void CheckScratchPercent()
    {
        float percent =
            (float)scratchedPixels /
            totalScratchPixels;

        if (percent >= clearPercent)
        {
            endingCalled = true;

            Debug.Log("복권 긁기 60% 달성!");

            if (scratchButtonManager != null)
            {
                Debug.Log("ScratchTicket → StartEndingFade 호출");
                scratchButtonManager.StartEndingFade();
            }
            else
            {
                Debug.LogError("ScratchTicket의 ScratchButtonManager가 연결되지 않았습니다.");
            }
        }
    }
}