using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader : SingletonMono<ScenesLoader>
{ 
    [SerializeField]
    string defaultScene;

    public void LoadDefaultScene()
    {
        LoadScene(defaultScene);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
