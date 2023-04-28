using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum SceneName : byte
{
    Start,
    MainMenu,
    SelectCharacter,
    Gameplay,
}

public class LoadingSceneManager : SingletorPersistent<LoadingSceneManager>
{
    public SceneName SceneActive => m_sceneActive;

    private SceneName m_sceneActive;
    // Start is called before the first frame update

    public void Init()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    public void LoadScene(SceneName sceneToLoad, bool isNetworkSessionActive = true)
    {
        StartCoroutine(Loading(sceneToLoad, isNetworkSessionActive));
    }

    private IEnumerator Loading(SceneName sceneToLoad, bool isNetworkSessionActive)
    {
        LoadingFadeEffect.Instance.FadeIn();

        yield return new WaitUntil(() => LoadingFadeEffect.s_canLoad);
        if (isNetworkSessionActive)
        {
            if (NetworkManager.Singleton.IsServer)
                LoadSceneNetwork(sceneToLoad);
        }
        else
        {
            LoadSceneLocal(sceneToLoad);
        }

        yield return new WaitForSeconds(1f);
        
        LoadingFadeEffect.Instance.FadeOut();
    }

    private void LoadSceneLocal(SceneName sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.ToString());
        switch (sceneToLoad)
        {
            case SceneName.MainMenu:
                
                break;
        }
    }
    
    private void LoadSceneNetwork(SceneName sceneToLoad)
    { 
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad.ToString(), LoadSceneMode.Single);
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // We only care the host/server is loading because every manager handles
        // their information and behavior on the server runtime
        if (!NetworkManager.Singleton.IsServer)
            return;

        Enum.TryParse(sceneName, out m_sceneActive);

        if (!ClientConnection.Instance.CanClientConnect(clientId))
            return;

        // What to initially do on every scene when it finishes loading
        switch (m_sceneActive)
        {
            // When a client/host connects tell the manager
            case SceneName.SelectCharacter:
                CharacterSelectionManager.Instance.ServerSceneInit(clientId);
                break;

            // When a client/host connects tell the manager to create the ship and change the music
            // case SceneName.Gameplay:
            //     GameplayManager.Instance.ServerSceneInit(clientId);
            //     break;


        }
    }
}
