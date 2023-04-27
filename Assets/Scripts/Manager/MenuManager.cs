using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private CharacterData[] m_characterDatas;

    [SerializeField] private SceneName nextScene = SceneName.SelectCharacter;

    private IEnumerator Start()
    {
        ClearAllCharacterData();

        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);
        
        LoadingSceneManager.Instance.Init();
    }
    
    private void ClearAllCharacterData()
    {
        // Clean the all the data of the characters so we can start with a clean slate
        foreach (CharacterData data in m_characterDatas)
        {
            data.EmptyData();
        }
    }

    public void OnClickCreate()
    {
        RelayController.Instance.CreateGame();
        
        LoadingSceneManager.Instance.LoadScene(nextScene);
    }

    public void OnClickConfirm()
    {
        RelayController.Instance.JoinGame(PlayerData.Instance.joinCode);
        
        LoadingSceneManager.Instance.LoadScene(nextScene);
    }
}
