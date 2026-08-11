using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool isLoading = false;

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }
}
