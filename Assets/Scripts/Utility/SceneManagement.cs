using System.Collections;
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
    //.
    //.
    // Add more scenes states if needed
}

public class SceneManagement : SingletorPersistent<SceneManagement>
{
    public SceneName SceneActive => m_sceneActive;

    private SceneName m_sceneActive;

    // After running the menu scene, which initiates this manager, we subscribe to these events
    // due to the fact that when a network session ends it cannot longer listen to them.
    public void Init()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    public void LoadScene(SceneName sceneToLoad, bool isNetworkSessionActive = true)
    {
        StartCoroutine(Loading(sceneToLoad, isNetworkSessionActive));
    }

    // Coroutine for the loading effect. It use an alpha in out effect
    private IEnumerator Loading(SceneName sceneToLoad, bool isNetworkSessionActive)
    {
        FadeEffect.Instance.FadeIn();

        // Here the player still sees the black screen
        yield return new WaitUntil(() => FadeEffect.s_canLoad);

        if (isNetworkSessionActive)
        {
            if (NetworkManager.Singleton.IsServer)
                LoadSceneNetwork(sceneToLoad);
        }
        else
        {
            LoadSceneLocal(sceneToLoad);
        }

        // Because the scenes are not heavy we can just wait a second and continue with the fade.
        // In case the scene is heavy instead we should use additive loading to wait for the
        // scene to load before we continue
        yield return new WaitForSeconds(1f);

        FadeEffect.Instance.FadeOut();
    }

    // Load the scene using the regular SceneManager, use this if there's no active network session
    private void LoadSceneLocal(SceneName sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.ToString());
        switch (sceneToLoad)
        {
            case SceneName.MainMenu:
                
            break;
        }
    }

    // Load the scene using the SceneManager from NetworkManager. Use this when there is an active
    // network session
    private void LoadSceneNetwork(SceneName sceneToLoad)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(
            sceneToLoad.ToString(),
            LoadSceneMode.Single);
    }

    // This callback function gets triggered when a scene is finished loading
    // Here we set up what to do for each scene, like changing the music
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
               //CharacterSelectionManager.Instance.ServerSceneInit(clientId);
                break;

            // When a client/host connects tell the manager to create the ship and change the music
            case SceneName.Gameplay:
                //GameplayManager.Instance.ServerSceneInit(clientId);
                break;

            
        }
    }
}
