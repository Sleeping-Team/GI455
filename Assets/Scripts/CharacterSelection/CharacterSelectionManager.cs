using System;
using TMPro;
using Unity.Netcode;
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
    public ConnectionState playerState;
    public PlayerCharSelection playerObject; 
    public string playerName;
    public ulong clientId;
}

[Serializable]
public struct CharacterContainer
{
    public CharacterSelection characterSelection;    
    public TextMeshProUGUI nameContainer;
    //public Image characterProfile;
    public GameObject readySign;
    //public GameObject arrow;                      
    //public GameObject arrowReady;                 
    //public GameObject arrowClient;              
    //public Image playerIcon;                       
    //public GameObject waitingText;                
    //public GameObject backgroundPlayerSelect;            
    //public TextMeshProUGUI namePlayerText;             
    //public GameObject backgroundPlayerReady;       
    //public TextMeshProUGUI namePlayerReadyText;       
    //public GameObject backgroundClientPlayerReady;   
    //public TextMeshProUGUI nameClientPlayerReadyText;
}
public class CharacterSelectionManager : SingletonNetwork<CharacterSelectionManager>
{
    public CharacterData[] charactersData;
    
    [SerializeField]
    GameObject m_readyButton;

    [SerializeField]
    GameObject m_cancelButton;

    [SerializeField]
    float m_timeToStartGame;

    [SerializeField]
    SceneName m_nextScene = SceneName.Gameplay;

    [SerializeField]
    Color m_clientColor;

    [SerializeField]
    Color m_playerColor;

    [SerializeField]
    PlayerConnectionState[] m_playerStates;

    [SerializeField]
    GameObject m_playerPrefab;
    
    [SerializeField]
    CharacterContainer[] m_charactersContainers;

    bool m_isTimerOn;
    float m_timer;

    private readonly Color k_selectedColor = new Color32(74, 74, 74, 255);

    // Start is called before the first frame update
    void Start()
    {
        m_timer = m_timeToStartGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;

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
    void OnDisable()
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
    void UpdatePlayerStateClientRpc(ulong clientId, int stateIndex, ConnectionState state,string name)
    {
        if (IsServer)
            return;

        m_playerStates[stateIndex].playerState = state;
        m_playerStates[stateIndex].clientId = clientId;
        m_playerStates[stateIndex].playerName = name;
    }

    void RemoveSelectedStates()
    {
        for (int i = 0; i < charactersData.Length; i++)
        {
            charactersData[i].isSelected = false;
        }
    }
    void RemoveReadyStates(ulong clientId, bool disconected)
    {
        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].playerState == ConnectionState.ready &&
                m_playerStates[i].clientId == clientId)
            {

                if (disconected)
                {
                    m_playerStates[i].playerState = ConnectionState.disconnected;
                    UpdatePlayerStateClientRpc(clientId, i, ConnectionState.disconnected,null);
                }
                else
                {
                    m_playerStates[i].playerState = ConnectionState.connected;
                    UpdatePlayerStateClientRpc(clientId, i, ConnectionState.connected,PlayerData.Instance.playerName);
                }
            }
        }
    }

    void StartGameTimer()
    {
        foreach (PlayerConnectionState state in m_playerStates)
        {
            // If a player is connected (not ready)
            if (state.playerState == ConnectionState.connected)
                return;
        }

        // If all players connected are ready
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
        m_charactersContainers[playerId].nameContainer.text = "Waiting...";
        //m_charactersContainers[playerId].characterProfile.sprite = null;
        //m_charactersContainers[playerId].characterProfile.color = new Color(225, 255, 255);
    }

    public ConnectionState GetConnectionState(int playerId)
    {
        if (playerId != -1)
            return m_playerStates[playerId].playerState;

        return ConnectionState.disconnected;
    }

    public void SetCharacterColor(int playerId, int characterSelected)
    {
        if (charactersData[characterSelected].isSelected)
        {
            //m_charactersContainers[playerId].nameContainer.color = k_selectedColor;
            m_charactersContainers[playerId].characterSelection.ShowLight(true);
        }
        else
        {
            //m_charactersContainers[playerId].nameContainer.color = Color.white;
            m_charactersContainers[playerId].characterSelection.ShowLight(false);
        }
        m_charactersContainers[playerId].characterSelection.SetPlayableChar(characterSelected);
    }

    public void SetCharacterUI(int playerId, int characterSelected)
    {
        m_charactersContainers[playerId].nameContainer.text = PlayerData.Instance.playerName;
        
        //m_charactersContainers[playerId].namePlayerText.text = charactersData[characterSelected].characterName;

        //m_charactersContainers[playerId].namePlayerReadyText.text = charactersData[characterSelected].characterName;

        //m_charactersContainers[playerId].nameClientPlayerReadyText.text = charactersData[characterSelected].characterName;
        
        SetCharacterColor(playerId, characterSelected);
    }

    public void SetPlayableChar(int playerId, int characterSelected, bool isClientOwner)
    {
        SetCharacterUI(playerId, characterSelected);
        //m_charactersContainers[playerId].playerIcon.gameObject.SetActive(true);
        if (isClientOwner)
        {
            //m_charactersContainers[playerId].arrowClient.SetActive(true);
            //m_charactersContainers[playerId].arrow.SetActive(false);
            //m_charactersContainers[playerId].arrowReady.SetActive(false);
            //m_charactersContainers[playerId].playerIcon.color = m_clientColor;
        }
        else
        {
            //m_charactersContainers[playerId].arrow.SetActive(true);
            //m_charactersContainers[playerId].arrowReady.SetActive(false);
            //m_charactersContainers[playerId].arrowClient.SetActive(false);
            //m_charactersContainers[playerId].playerIcon.color = m_playerColor;
        }

        m_charactersContainers[playerId].characterSelection.SetPlayableChar(characterSelected);
        //m_charactersContainers[playerId].backgroundPlayerSelect.SetActive(true);
        //m_charactersContainers[playerId].waitingText.SetActive(false);
    }

    [ClientRpc]
    void PlayerConnectsClientRpc(
        ulong clientId,
        int stateIndex,
        ConnectionState state,
        NetworkObjectReference player)
    {
        if (IsServer)
            return;

        if (state != ConnectionState.disconnected)
        {
            m_playerStates[stateIndex].playerState = state;
            m_playerStates[stateIndex].clientId = clientId;

            if (player.TryGet(out NetworkObject playerObject))
                m_playerStates[stateIndex].playerObject =
                    playerObject.GetComponent<PlayerCharSelection>();
        }
    }

    public void ServerSceneInit(ulong clientId)
    {
        GameObject go =
            NetworkObjectSpawner.SpawnNewNetworkObjectChangeOwnershipToClient(
                m_playerPrefab,
                transform.position,
                clientId,
                true);

        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].playerState == ConnectionState.disconnected)
            {
                m_playerStates[i].playerState = ConnectionState.connected;
                m_playerStates[i].playerObject = go.GetComponent<PlayerCharSelection>();
                m_playerStates[i].playerName = go.name;
                m_playerStates[i].clientId = clientId;

                // Force the exit
                break;
            }
        }

        // Sync states to clients
        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].playerObject != null)
                PlayerConnectsClientRpc(
                    m_playerStates[i].clientId,
                    i,
                    m_playerStates[i].playerState,
                    m_playerStates[i].playerObject.GetComponent<NetworkObject>());
        }

    }
    public int GetPlayerId(ulong clientId)
    {
        for (int i = 0; i < m_playerStates.Length; i++)
        {
            if (m_playerStates[i].clientId == clientId)
                return i;
        }

        //! This should never happen
        Debug.LogError("This should never happen");
        return -1;
    }

    [ClientRpc]
    void PlayerReadyClientRpc(ulong clientId, int playerId, int characterSelected,string name)
    {
        charactersData[characterSelected].isSelected = true;
        charactersData[characterSelected].clientId = clientId;
        charactersData[characterSelected].playerId = playerId;
        charactersData[characterSelected].characterName = name;
        m_playerStates[playerId].playerState = ConnectionState.ready;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            m_charactersContainers[playerId].readySign.SetActive(true);
            //m_charactersContainers[playerId].backgroundClientPlayerReady.SetActive(true);
            //m_charactersContainers[playerId].backgroundPlayerSelect.SetActive(false);
        }
        else
        {
            m_charactersContainers[playerId].readySign.SetActive(true);
            //m_charactersContainers[playerId].arrow.SetActive(false);
            //m_charactersContainers[playerId].arrowReady.SetActive(true);
            //m_charactersContainers[playerId].backgroundPlayerSelect.SetActive(false);
            //m_charactersContainers[playerId].backgroundPlayerReady.SetActive(true);
        }

        for (int i = 0; i < m_playerStates.Length; i++)
        {
            // Only changes the ones on clients that are not selected            
            if (m_playerStates[i].playerState == ConnectionState.connected)
            {
                if (m_playerStates[i].playerObject.CharSelected == characterSelected)
                {
                    SetCharacterColor(i, characterSelected);
                }
            }
        }

       // AudioManager.Instance.PlaySoundEffect(m_confirmClip);
    }

    [ClientRpc]
    void PlayerNotReadyClientRpc(ulong clientId, int playerId, int characterSelected)
    {
        charactersData[characterSelected].isSelected = false;
        charactersData[characterSelected].clientId = 0UL;
        charactersData[characterSelected].playerId = -1;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            m_charactersContainers[playerId].readySign.SetActive(false);
            //m_charactersContainers[playerId].arrowClient.SetActive(true);
            //m_charactersContainers[playerId].backgroundClientPlayerReady.SetActive(false);
            //m_charactersContainers[playerId].backgroundPlayerSelect.SetActive(true);
        }
        else
        {
            m_charactersContainers[playerId].readySign.SetActive(false);
            //m_charactersContainers[playerId].arrow.SetActive(true);
            //m_charactersContainers[playerId].arrowReady.SetActive(false);
            //m_charactersContainers[playerId].arrowClient.SetActive(false);
            //m_charactersContainers[playerId].backgroundPlayerSelect.SetActive(true);
            //m_charactersContainers[playerId].backgroundPlayerReady.SetActive(false);
        }

        
        for (int i = 0; i < m_playerStates.Length; i++)
        {
            // Only changes the ones on clients that are not selected
            if (m_playerStates[i].playerState == ConnectionState.connected)
            {
                if (m_playerStates[i].playerObject.CharSelected == characterSelected)
                {
                    SetCharacterColor(i, characterSelected);
                }
            }
        }
    }

    [ClientRpc]
    public void PlayerDisconnectedClientRpc(int playerId)
    {
        SetNonPlayableChar(playerId);

        // All character data unselected
        RemoveSelectedStates();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnects;
        }
    }

    // Set the player ready if the player is not selected and check if all player are ready to start the countdown
    public void PlayerReady(ulong clientId, int playerId, int characterSelected,string name)
    {
        if (!charactersData[characterSelected].isSelected)
        {
            PlayerReadyClientRpc(clientId, playerId, characterSelected, name);

            StartGameTimer();
        }
    }
    // Check if the player has selected the character
    public bool IsSelectedByPlayer(int playerId, int characterSelected)
    {
        return charactersData[characterSelected].playerId == playerId ? true : false;
    }

    // Set the players UI button
    public void SetPlayerReadyUIButtons(bool isReady, int characterSelected)
    {
        if (isReady && !charactersData[characterSelected].isSelected)
        {
            m_readyButton.SetActive(false);
            m_cancelButton.SetActive(true);
        }
        else if (!isReady && charactersData[characterSelected].isSelected)
        {
            m_readyButton.SetActive(true);
            m_cancelButton.SetActive(false);
        }
    }

    public void PlayerDisconnects(ulong clientId)
    {
        if (!ClientConnection.Instance.IsExtraClient(clientId))
            return;

        PlayerNotReady(clientId, isDisconected: true);

        m_playerStates[GetPlayerId(clientId)].playerObject.Despawn();

        // The client disconnected is the host
        if (clientId == 0)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void PlayerNotReady(ulong clientId, int characterSelected = 0, bool isDisconected = false)
    {
        int playerId = GetPlayerId(clientId);
        m_isTimerOn = false;
        m_timer = m_timeToStartGame;

        RemoveReadyStates(clientId, isDisconected);

        // Notify clients to change UI
        if (isDisconected)
        {
            PlayerDisconnectedClientRpc(playerId);
        }
        else
        {
            PlayerNotReadyClientRpc(clientId, playerId, characterSelected);
        }
    }
}
