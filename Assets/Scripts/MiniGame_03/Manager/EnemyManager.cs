using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public Slider hpSlider;

    [Header("체력 하트 진동")]
    public RectTransform heartImage;        //하트 이미지
    public float shakeDuration = 0.5f;      //하트 흔들리는 시간
    public float shakeAmount = 10f;         //하트가 좌우로 흔들리는 정도

    private Coroutine shakeCoroutine;
    private int maxHp = 100;
    private int currentHp;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHp = maxHp;

        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
    }

    //데미지 입었을 때
    public void Damage(int amount)
    {
        currentHp -= amount;

        if (currentHp < 0)
            currentHp = 0;
        
        if (hpSlider != null)
            hpSlider.value = currentHp;

        if (currentHp > 0)
        {
            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            shakeCoroutine = StartCoroutine(ShakeHeart());
        }

        if (currentHp == 0)
        {
            if (Game3Manager.Instance != null)
                Game3Manager.Instance.GameSuccess();
        }
    }

    //하트 진동
    private IEnumerator ShakeHeart()
    {
        if (heartImage == null)
            yield break;

        Vector2 originalPos = heartImage.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float x = Random.Range(-shakeAmount, shakeAmount);

            heartImage.anchoredPosition =
                originalPos + new Vector2(x, 0f);

            yield return null;
        }

        heartImage.anchoredPosition = originalPos;
        shakeCoroutine = null;
    }
}