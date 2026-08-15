using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public static WordSpawner Instance;

    [Header("고정 Word Buttons")]
    public WordButton[] wordButtons = new WordButton[7];

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnWords(string shuffledWord)
    {
        for (int i = 0; i < wordButtons.Length; i++)
        {
            if (wordButtons[i] == null)
            {
                Debug.LogError($"WordButton {i + 1}이 연결되지 않았습니다.");
                continue;
            }

            if (i < shuffledWord.Length)
            {
                wordButtons[i].gameObject.SetActive(true);
                wordButtons[i].Initialize(shuffledWord[i]);
            }
            else
            {
                wordButtons[i].gameObject.SetActive(true);
                wordButtons[i].SetEmpty();
            }
        }
    }

    public void RestoreAll()
    {
        foreach (WordButton button in wordButtons)
        {
            if (button.Letter != '\0')
                button.Restore();
        }
    }
}