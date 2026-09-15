using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    public string sceneName;

    public void ChangeScene()
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
