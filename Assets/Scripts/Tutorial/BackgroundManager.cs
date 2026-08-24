using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite Background;
    [SerializeField] private Sprite streetBackground;
    [SerializeField] private Sprite cafeBackground;

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

    public void ChangeToBackground()
    {
        backgroundImage.sprite = Background;
    }
}