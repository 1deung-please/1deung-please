using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite streetBackground;
    [SerializeField] private Sprite cafeBackground;
    [SerializeField] private Sprite Background_Timer;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeToStreet()
    {
        backgroundImage.sprite = streetBackground;
    }

    public void ChangeToCafe()
    {
        backgroundImage.sprite = cafeBackground;
    }
    
    public void ChangeToTimer()
    {
        backgroundImage.sprite = Background_Timer;
    }
}