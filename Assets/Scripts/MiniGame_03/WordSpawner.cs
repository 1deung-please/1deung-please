using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public static WordSpawner Instance;

    public Transform wordPanel;

    public GameObject wordButtonPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnWords(string shuffled)
    {
        Debug.Log(shuffled);

        foreach (Transform child in wordPanel)
            Destroy(child.gameObject);

        foreach (char c in shuffled)
        {
            Debug.Log(c);
            GameObject obj =
                Instantiate(wordButtonPrefab, wordPanel);

            obj.GetComponent<WordButton>()
                .Initialize(c.ToString());
        }
    }
}