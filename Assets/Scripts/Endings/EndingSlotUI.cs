using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingSlotUI : MonoBehaviour
{
    public Image badgeImage;
    public TMP_Text nameText;

    private string sceneName;
    private bool isUnlocked;

    public void Setup(EndingInfo info, bool unlocked)
    {
        sceneName = info.sceneName;
        isUnlocked = unlocked;

        if (unlocked)
        {
            nameText.text = info.title;
            if (info.badge != null) badgeImage.sprite = info.badge;
            badgeImage.color = Color.white;
        }
        else
        {
            nameText.text = "???";
            badgeImage.color = Color.black; // 실루엣 대용
        }
    }

    // 슬롯 버튼의 OnClick에 연결
    public void OnClickReplay()
    {
        if (!isUnlocked) return;
        SceneLoader.Instance.LoadScene(sceneName);
    }
}