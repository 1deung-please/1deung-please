using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public Slider hpSlider;

    private int maxHp = 100;
    private int currentHp;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHp = maxHp;

        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
    }

    public void Damage(int amount)
    {
        currentHp -= amount;

        if (currentHp < 0)
            currentHp = 0;

        hpSlider.value = currentHp;

        if(currentHp == 0)
        {
            Debug.Log("게임 성공!");
        }
    }
}