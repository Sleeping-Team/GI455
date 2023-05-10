using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


public enum ConnectionState : byte
{
    connected,
    disconnected,
    ready
}

[Serializable]
public struct PlayerConnectionState
{
    public ConnectionState PlayerState;
    public PlayerCharSelection playerObject;
    public string playerName;
    public ulong clientId;
}

[Serializable]
public struct CharacterContainer
{
    public CharacterSelection characterSelection;
    public TextMeshProUGUI nameContainer;
    public GameObject readyIcon;
}

public class CharacterSelectionManager : SingletonNetwork<CharacterSelectionManager>
{
    public CharacterData[] charactersData;

    [SerializeField] private CharacterContainer[] m_charactersContainers;
    
    [SerializeField] private GameObject m_readyButton;
    [SerializeField] private GameObject m_cancleButton;

    [SerializeField] private float m_timeToStartGame;
    [SerializeField] private SceneName m_nextScene = SceneName.Load;

    [SerializeField] private PlayerConnectionState[] m_playerStates;

    [SerializeField] private GameObject m_playerPrefab;

    private bool m_isTimerOn;
    private float m_timer;

    void Start()
    {
        m_timer = m_timeToStartGame;
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (m_isTimerOn)
        {
            m_timer -= Time.deltaTime;

            if (m_timer < 0f)
            {
                m_isTimerOn = false;
                StartGame();
            }
        }
    }

    private void OnDisable()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnects;
        }
    }

    void StartGame()
    {
        StartGameClientRpc();
        LoadingSceneManager.Instance.LoadScene(m_nextScene);
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        LoadingFadeEffect.Instance.FadeAll();
    }

    [ClientRpc]
    public void UpdatePlayerStateClientRpc(ulong clientId, int stateIndex, ConnectionState state,string name)
    {
        if (IsServer)
        {
            return;
        }

        m_playerStates[stateIndex].PlayerState = state;
        m_playerStates[stateIndex].clientId = clientId;
        m_playerStates[stateIndex].playerName = name;
    }

    void RemoveSelectedStates()
    {
        for (int i = 0; i < charactersData.Length ; i++)
        {
            charactersData[i].isSelected = false;
        }
    }

    void RemoveReadyStates(ulong clientId, bool disconnected )
    {
        for (int i = 0; i < m_playerStates.Length ; i++)
        {
            if (m_playerStates[i].PlayerState == ConnectionState.ready &&
                m_playerStates[i].clientId == clientId)
            {
                if (disconnected)
                {
                    m_playerStates[i].PlayerState = ConnectionState.disconnected;
                    UpdatePlayerStateClientRpc(clientId,i,ConnectionState.disconnected,"waiting");
                }
                else
                {
                    m_playerStates[i].PlayerState = ConnectionState.connected;
                    UpdatePlayerStateClientRpc(clientId,i,ConnectionState.connected,m_playerStates[clientId].playerName);
                }
            }
        }
    }

    void StartGameTimer()
    {
        foreach (PlayerConnectionState state in m_playerStates)
        {
            if (state.PlayerState == ConnectionState.connected)
            {
                return;
            }
        }

        m_timer = m_timeToStartGame;
        m_isTimerOn = true;
    }

    public bool IsReady(int playerId)
    {
        return charactersData[playerId].isSelected;
    }

    void SetNonPlayableChar(int playerId)
    {
        m_charactersContainers[playerId].characterSelection.SetNonPlayableChar();
        m_charactersContainers[playerId].nameContainer.text = "waiting";
    }

    public ConnectionState GetConnectionState(int playerId)
    {
        if (playerId != -1)
        {
            return m_playerStates[playerId].PlayerState;
        }

        return ConnectionState.disconnected;
    }

    public void SetCharacterColor(int playerId, int characterSelected)
    {
        if (charactersData[characterSelected].isSelected)
        {
            //can not select THIS character
            m_charactersContainers[playerId].characterSelection.ShowLight(true);
        }
        else
        {
            //can select THIS character
            m_charactersContainers[playerId].characterSelection.ShowLight(false);
        }
        m_charactersContainers[playerId].characterSelection.SetPlayableChar(characterSelected);
    }

    public void SetCharacterUI(int playerId, int characterSelected)
    {
        //m_charactersContainers[playerId].nameContainer.text = PlayerData.Instance.playerName;
        
        SetCharacterColor(playerId,characterSelected);
    }

    public void SetPlayebleChar(int playerId, int characterSelected, bool isClientOwner,string name)
    {
        SetCharacterUI(playerId,characterSelected);
        
        m_charactersContainers[playerId].readyIcon.SetActive(false);
        
        m_charactersContainers[playerId].nameContainer.text = name;
        
        m_charactersContainers[playerId].characterSelection.SetPlayableChar(characterSelected);
    }

    public void ChangePlayerName(int playerId, string name)
    {
        m_charactersContainers[playerId].nameContainer.text = name;
    }
    
    [ClientRpc]
    void ChangeNameClientRpc(ulong clientId, int stateIndex, ConnectionState state, string name)
    {
        m_playerStates[stateIndex].playerName = name;
    }

    [ClientRpc]
    void PlayerConnectsClientRpc(ulong clientId, int stateIndex, ConnectionState state, string name,
        NetworkObjectReference player)
    {
        if (IsServer)
        {
            return;
        }

        if (state != ConnectionState.disconnected)
        {
            m_playerStates[stateIndex].PlayerState = state;
            m_playerStates[stateIndex].clientId = clientId;
            m_playerStates[stateIndex].playerName = name;

            if (player.TryGet(out NetworkObject playerObject))
            {
                m_playerStates[stateIndex].playerObject = playerObject.GetComponent<PlayerCharSelection>();
            }
        }
    }

    public void ServerSceneInit(ulong clientId)
    {
        GameObject go =
            NetworkObjectSpawner.SpawnNewPlayerObjectToClient(m_playerPrefab, transform.position,
                clientId, true);

        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].PlayerState == ConnectionState.disconnected)
            {
                m_playerStates[i].PlayerState = ConnectionState.connected;
                m_playerStates[i].playerObject = go.GetComponent<PlayerCharSelection>();
                m_playerStates[i].playerName = go.name;
                m_playerStates[i].clientId = clientId;
                
                break;
            }
        }

        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].playerObject != null)
            {
                PlayerConnectsClientRpc(m_playerStates[i].clientId,
                    i,
                    m_playerStates[i].PlayerState,
                    m_playerStates[i].playerName,
                    m_playerStates[i].playerObject.GetComponent<NetworkObject>());
            }
        }
    }

    public int GetPlayerId(ulong clientId)
    {
        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].clientId == clientId)
            {
                return i;
            }
        }

        Debug.LogError("This should never happen");
        return -1;
    }

    [ClientRpc]
    void PlayerReadyClientRpc(ulong clientId, int playerId, int characterSelected)
    {
        charactersData[characterSelected].isSelected = true;
        charactersData[characterSelected].clientId = clientId;
        charactersData[characterSelected].playerId = playerId;
        m_playerStates[playerId].PlayerState = ConnectionState.ready;
        
        m_charactersContainers[playerId].readyIcon.SetActive(true);

        for (int i = 0; i < m_playerStates.Length ; i++)
        {
            if (m_playerStates[i].PlayerState == ConnectionState.connected)
            {
                if (m_playerStates[i].playerObject.CharSelected == characterSelected)
                {
                    SetCharacterColor(i,characterSelected);
                }
            }
        }
    }

    [ClientRpc]
    void PlayerNotReadyClientRpc(ulong clientId, int playerId, int characterSelected)
    {
        charactersData[characterSelected].isSelected = false;
        charactersData[characterSelected].clientId = 0UL;
        charactersData[characterSelected].playerId = -1;
        
        m_charactersContainers[playerId].readyIcon.SetActive(false);

        for (int i = 0; i < m_playerStates.Length ; i++)
        {
            if (m_playerStates[i].PlayerState == ConnectionState.connected)
            {
                if (m_playerStates[i].playerObject.CharSelected == characterSelected)
                {
                    SetCharacterColor(i,characterSelected);
                }
            }
        }
    }
    
    [ClientRpc]
    public void PlayerDisconnectedClientRpc(int playerId)
    {
        SetNonPlayableChar(playerId);
        
        RemoveSelectedStates();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnects;
        }
    }

    public void PlayerReady(ulong clientId, int playerId, int characterSelected)
    {
        if (!charactersData[characterSelected].isSelected)
        {
            PlayerReadyClientRpc(clientId,playerId,characterSelected);
            
            StartGameTimer();
        }
    }

    public bool IsSelectedByPlayer(int playerId, int characterSeleted)
    {
        return charactersData[characterSeleted].playerId == playerId ? true : false;
    }

    public void SetPlayerReadyUIButtons(bool isReady, int characterSelected)
    {
        if (isReady && !charactersData[characterSelected].isSelected)
        {
            m_readyButton.SetActive(false);
            m_cancleButton.SetActive(true);
        }
        else if (!isReady && charactersData[characterSelected].isSelected)
        {
            m_readyButton.SetActive(true);
            m_cancleButton.SetActive(false);
        }
    }

    public void PlayerDisconnects(ulong clientId)
    {
        if (!ClientConnection.Instance.IsExtraClient(clientId))
        {
            return;
        }

        PlayerNotReady(clientId, isDisconnected: true);

        m_playerStates[GetPlayerId(clientId)].playerObject.Despawn();

        if (clientId == 0)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void PlayerNotReady(ulong clientId, int characterSelected = 0, bool isDisconnected = false)
    {
        int playerId = GetPlayerId(clientId);
        m_isTimerOn = false;
        m_timer = m_timeToStartGame;
        
        RemoveReadyStates(clientId,isDisconnected);

        if (isDisconnected)
        {
            PlayerDisconnectedClientRpc(playerId);
        }
        else
        {
            PlayerNotReadyClientRpc(clientId,playerId,characterSelected);
        }
    }
}
