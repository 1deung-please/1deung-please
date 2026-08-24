using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public static WordSpawner Instance;

    [Header("고정 Word Buttons")]
    public WordButton[] wordButtons = new WordButton[7];    //7개 단어 버튼

    private void Awake()
    {
        Instance = this;
    }

    //글자들을 7개 버튼에 배치
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

    //취소 버튼 눌렀을 때 복구 과정
    public void RestoreAll()
    {
        foreach (WordButton button in wordButtons)
        {
            if (button.Letter != '\0')
                button.Restore();
        }
    }
}