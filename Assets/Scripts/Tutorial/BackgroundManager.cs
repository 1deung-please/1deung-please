using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [SerializeField] private SpriteRenderer backgroundRenderer;

    [SerializeField] private Sprite streetBackground;
    [SerializeField] private Sprite cafeBackground;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeToStreet()
    {
        backgroundRenderer.sprite = streetBackground;
    }

    public void ChangeToCafe()
    {
        backgroundRenderer.sprite = cafeBackground;
    }
}