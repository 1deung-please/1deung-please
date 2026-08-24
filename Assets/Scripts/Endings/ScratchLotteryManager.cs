using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScratchLotteryManager : MonoBehaviour
{
    [Header("Lottery UI")]
    [SerializeField] private GameObject lotteryPanel;

    [Header("Scratch")]
    [SerializeField] private RawImage scratchArea;
    [SerializeField] private Texture2D scratchTexture;

    [Header("Cursor")]
    [SerializeField] private Texture2D coinCursor;

    [Header("Scratch Settings")]
    [SerializeField] private int brushSize = 40;
    [SerializeField] private float requiredPercent = 60f;

    [Header("Ending")]
    [SerializeField] private EndingManager endingManager;
    [SerializeField] private CanvasGroup fadePanel;

    private Texture2D runtimeTexture;
    private RectTransform scratchRect;

    private bool[] validPixels;
    private bool[] erasedPixels;
    private int validPixelCount;
    private int erasedPixelCount;

    private bool isDragging = false;
    private bool endingStarted = false;

    private void Start()
    {
        // 처음에는 복권 숨김
        if (lotteryPanel != null)
        {
            lotteryPanel.SetActive(false);
        }

        if (scratchArea == null)
        {
            Debug.LogError("ScratchArea가 연결되지 않았습니다!");
            return;
        }

        if (scratchTexture == null)
        {
            Debug.LogError("ScratchTexture가 연결되지 않았습니다!");
            return;
        }

        // 원본 이미지가 읽을 수 있는지 확인
        if (!scratchTexture.isReadable)
        {
            Debug.LogError(
                "ScratchTexture의 Read/Write가 꺼져 있습니다!"
            );
            return;
        }

        scratchRect = scratchArea.rectTransform;

        // 원본을 복사해서 실제로 지울 수 있는 텍스처 생성
        runtimeTexture = new Texture2D(
            scratchTexture.width,
            scratchTexture.height,
            TextureFormat.RGBA32,
            false
        );

        Color[] sourcePixels = scratchTexture.GetPixels();

        // 원래 이미지의 각 픽셀 정보를 저장
        validPixels = new bool[sourcePixels.Length];
        erasedPixels = new bool[sourcePixels.Length];

        // 원래 불투명했던 픽셀만 개수에 포함
        for (int i = 0; i < sourcePixels.Length; i++)
        {
            if (sourcePixels[i].a > 0.1f)
            {
                validPixels[i] = true;
                validPixelCount++;
            }
        }

        // 복사한 이미지 생성
        runtimeTexture.SetPixels(sourcePixels);
        runtimeTexture.Apply();

        // 실제 스크래치 이미지로 사용
        scratchArea.texture = runtimeTexture;
    }

    private void Update()
    {
        if (endingStarted)
            return;

        if (lotteryPanel == null || !lotteryPanel.activeSelf)
            return;

        // 마우스 버튼 누르기 시작
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;

            ScratchAtMouse();
        }

        // 마우스를 누른 상태에서 이동
        if (Input.GetMouseButton(0) && isDragging)
        {
            ScratchAtMouse();
        }

        // 마우스 버튼 떼기
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void ScratchAtMouse()
    {
        Vector2 localPosition;

        // Canvas의 카메라 가져오기
        Canvas canvas = scratchArea.canvas;

        Camera cam = null;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }

        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            scratchRect,
            Input.mousePosition,
            cam,
            out localPosition
        );

        if (!inside)
            return;

        float width = scratchRect.rect.width;
        float height = scratchRect.rect.height;

        // UI 좌표 → 텍스처 좌표
        float normalizedX =
            (localPosition.x + width / 2f) / width;

        float normalizedY =
            (localPosition.y + height / 2f) / height;

        if (normalizedX < 0f || normalizedX > 1f ||
            normalizedY < 0f || normalizedY > 1f)
        {
            return;
        }

        int pixelX = Mathf.Clamp(
            Mathf.FloorToInt(normalizedX * runtimeTexture.width),
            0,
            runtimeTexture.width - 1
        );

        int pixelY = Mathf.Clamp(
            Mathf.FloorToInt(normalizedY * runtimeTexture.height),
            0,
            runtimeTexture.height - 1
        );

        EraseCircle(pixelX, pixelY);

        runtimeTexture.Apply();

        CheckScratchPercent();
    }

    private void EraseCircle(int centerX, int centerY)
    {
        int radius = brushSize / 2;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                // 원 모양 브러시
                if (x * x + y * y > radius * radius)
                    continue;

                int pixelX = centerX + x;
                int pixelY = centerY + y;

                if (pixelX < 0 || pixelX >= runtimeTexture.width ||
                    pixelY < 0 || pixelY >= runtimeTexture.height)
                {
                    continue;
                }

                int index = pixelY * runtimeTexture.width + pixelX;

                // 원래 불투명했던 픽셀만 긁은 비율에 포함
                if (validPixels[index] && !erasedPixels[index])
                {
                    erasedPixels[index] = true;
                    erasedPixelCount++;
                }

                // 화면에서는 투명하게 만들기
                runtimeTexture.SetPixel(
                    pixelX,
                    pixelY,
                    Color.clear
                );
            }
        }
    }

    private void CheckScratchPercent()
    {
        if (validPixelCount <= 0)
            return;

        float percent =
            erasedPixelCount / (float)validPixelCount * 100f;

        Debug.Log(
            "복권 긁은 정도 : " +
            percent.ToString("F1") +
            "%"
        );

        if (percent >= requiredPercent)
        {
            StartEnding();
        }
    }

    // 대화가 끝났을 때 호출
    public void ShowLottery()
    {
        if (lotteryPanel == null)
        {
            Debug.LogError("LotteryPanel이 연결되지 않았습니다!");
            return;
        }

        lotteryPanel.SetActive(true);

        // 복권이 나타나는 순간 동전 커서
        if (coinCursor != null)
        {
            Cursor.SetCursor(
                coinCursor,
                new Vector2(
                    coinCursor.width / 2f,
                    coinCursor.height / 2f
                ),
                CursorMode.Auto
            );
        }

        Debug.Log("복권 등장 → 동전 커서");
    }

    private void StartEnding()
    {
        if (endingStarted)
            return;

        endingStarted = true;

        Debug.Log("복권 60% 긁음 → 엔딩 진입");

        StartCoroutine(FadeAndEnding());
    }

    private IEnumerator FadeAndEnding()
    {
        float time = 0f;

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime;

                fadePanel.alpha = time;

                yield return null;
            }
        }

        // 동전 커서 원래대로
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (endingManager != null)
        {
            endingManager.DetermineEnding();
        }
        else
        {
            Debug.LogError("EndingManager가 연결되지 않았습니다!");
        }
    }
}