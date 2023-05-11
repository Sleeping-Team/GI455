using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName : byte
{
    Start,
    MainMenu,
    SelectCharacter,
    Gameplay,
    Load
}

public class LoadingSceneManager : SingletorPersistent<LoadingSceneManager>
{
    public SceneName SceneActive => m_sceneActive;

    private SceneName m_sceneActive;

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
            {
                LoadSceneNetwork(sceneToLoad);
            }
            else
            {
                LoadSceneLocal(sceneToLoad);
            }
        }
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
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        Enum.TryParse(sceneName, out m_sceneActive);

        if (!ClientConnection.Instance.CanClientConnect(clientId))
        {
            return;
        }

        switch (m_sceneActive)
        {
            case SceneName.SelectCharacter:
                CharacterSelectionManager.Instance.ServerSceneInit(clientId);
                break;
            
            case SceneName.Gameplay:
                GameplayManager.Instance.ServerSceneInit(clientId);
                break;
        }
    }
}
