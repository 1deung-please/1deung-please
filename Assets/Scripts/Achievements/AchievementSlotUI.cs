using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementSlotUI : MonoBehaviour
{
    public Image badgeImage;
    public TMP_Text nameText;

    public void Setup(AchievementInfo info, bool unlocked)
    {
        if (unlocked)
        {
            nameText.text = info.title;
            if (info.badge != null) badgeImage.sprite = info.badge;
            badgeImage.color = Color.white;
        }
        else
        {
            nameText.text = "???";
            badgeImage.color = Color.black; // 실루엣 대용 (임시)
        }
    }
}