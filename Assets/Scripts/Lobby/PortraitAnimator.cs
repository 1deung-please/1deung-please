using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortraitAnimator : MonoBehaviour
{
    [Header("UI Image Target")]
    [SerializeField] private Image portraitImage;

    [Header("Sprite Animation Frames (5장)")]
    [SerializeField] private Sprite[] animationFrames; 

    [Header("Animation Settings")]
    [SerializeField] private float frameRate = 0.1f;  
    [SerializeField] private bool playOnlyOnEnable = true;

    private Coroutine animCoroutine;

    private void Awake()
    {
        if (portraitImage == null)
            portraitImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public void StartAnimation()
    {
        StopAnimation();

        if (animationFrames != null && animationFrames.Length > 0)
        {
            animCoroutine = StartCoroutine(PlayAnimationRoutine());
        }
    }

    public void StopAnimation()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }

    private IEnumerator PlayAnimationRoutine()
    {
        int index = 0;

        while (true)
        {
            if (portraitImage != null && animationFrames.Length > 0)
            {
                portraitImage.sprite = animationFrames[index];
                index = (index + 1) % animationFrames.Length; // 0~4 무한 반복
            }

            yield return new WaitForSeconds(frameRate);
        }
    }
}