using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite streetBackground;
    [SerializeField] private Sprite cafeBackground;
    [SerializeField] private Sprite Background_Timer;
    [SerializeField] private Sprite tutorialBackground;
    [SerializeField] private Sprite tutorial_bus_stop;
    [SerializeField] private Sprite tutorial_underground_shopping_center;
    [SerializeField] private Sprite tutorial_cafe;
    [SerializeField] private Sprite tutorial_bag;

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

    public void ChangeToTutorial()
    {
        backgroundImage.sprite = tutorialBackground;
    }

    public void ChangeToTutorial_bus_stop()
    {
        backgroundImage.sprite = tutorial_bus_stop;
    }

    public void ChangeToTutorial_underground_shopping_center()
    {
        backgroundImage.sprite = tutorial_underground_shopping_center;
    }

    public void ChangeToTutorial_cafe()
    {
        backgroundImage.sprite = tutorial_cafe;
    }

    public void ChangeToTutorial_bag()
    {
        backgroundImage.sprite = tutorial_bag;
    }
    
    public void ChangeToStreet_Normal()
    {
        backgroundImage.sprite = streetBackground;
    }
}