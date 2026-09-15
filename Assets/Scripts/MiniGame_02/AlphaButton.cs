using UnityEngine;
using UnityEngine.UI;

public class AlphaButton : MonoBehaviour
{
    [SerializeField] float alphaThreshold = 0.1f; 
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}