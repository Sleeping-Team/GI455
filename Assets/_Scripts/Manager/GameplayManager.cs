using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameplayManager : SingletonNetwork<GameplayManager>
{
    [SerializeField] private CharacterData[] m_charactersData;

    [SerializeField] private Transform[] m_playerStartingPositions;

    private int m_numberOfPlayerConnected;

    private List<ulong> m_connectedClients = new List<ulong>();

    private List<PlayerMovement> m_players = new List<PlayerMovement>();

    private void OnEnable()
    {
        if (!IsServer)
        {
            return;
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClientDisconnect(ulong clientId)
    {
        foreach (var player in m_players)
        {
            if (player != null)
            {
                
            }
        }
    }

    private IEnumerator HostShutdown()
    {
        ShutdownClientRpc();

        yield return new WaitForSeconds(0.5f);
        
        Shutdown();
    }

    [ClientRpc]
    private void LoadClientRpc()
    {
        if (IsServer)
        {
            return;
        }
        
        LoadingFadeEffect.Instance.FadeAll();
    }

    private void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        
        LoadingSceneManager.Instance.LoadScene(SceneName.MainMenu,false);
    }

    [ClientRpc]
    private void ShutdownClientRpc()
    {
        if (IsServer)
        {
            return;
        }
        
        Shutdown();
    }

    public void ExitToMenu()
    {
        if (IsServer)
        {
            StartCoroutine(HostShutdown());
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            LoadingSceneManager.Instance.LoadScene(SceneName.MainMenu, false);
        }
    }

    public void ServerSceneInit(ulong clientId)
    {
        Debug.LogWarning("Louise แกงกู");
        
        m_connectedClients.Add(clientId);

        if (m_connectedClients.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return;
        }

        foreach (var client in m_connectedClients)
        {
            int index = 0;

            foreach (CharacterData data in m_charactersData)
            {
                if (data.isSelected && data.clientId == client)
                {
                    GameObject player =
                        NetworkObjectSpawner.SpawnNewPlayerObjectToClient(
                            data.characterPrefab,
                            m_playerStartingPositions[m_numberOfPlayerConnected].position,
                            data.clientId,
                            true);
                    
                    PlayerMovement playerController =
                        player.GetComponent<PlayerMovement>();
                    

                    m_players.Add(playerController);
                    
                    m_numberOfPlayerConnected++;
                }

                index++;
            }
        }
    }
}
