using System;
using System.Collections;
using FlatKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NetworkManager = Unity.Netcode.NetworkManager;

public class LoadingScene : MonoBehaviour
{
    //public Image loadingBar;
    
    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        Debug.Log("As it's work, it's work");
        AsyncOperation asyncLoad;

        asyncLoad = SceneManager.LoadSceneAsync("_Scenes/Gameplay");
        
        while (!asyncLoad.isDone)
        {
            //float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            //loadingBar.fillAmount = progress;
            yield return null;
        }
    }
}
