using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharSelection : NetworkBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Space] 
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button cancleButton;

    [Space] 
    [SerializeField] private GameObject readyUI;
    [SerializeField] private GameObject cancleUI;
    
    [Space]
    
    [SerializeField] private TMP_Text roomCode;
    
    private const int k_noCharacterSelectionValue = -1;

    [SerializeField]
    private NetworkVariable<int> m_charSelected = new NetworkVariable<int>(k_noCharacterSelectionValue);

    [SerializeField] 
    private NetworkVariable<int> m_playerId = new NetworkVariable<int>(k_noCharacterSelectionValue);
    
    public int CharSelected => m_charSelected.Value;

    private bool HasSelectCharacter()
    {
        return m_playerId.Value != k_noCharacterSelectionValue;
    }

    void Start()
    {
        if (IsServer)
        {
            m_playerId.Value = CharacterSelectionManager.Instance.GetPlayerId(OwnerClientId);
        }
        else if (!IsOwner && HasSelectCharacter())
        {
            CharacterSelectionManager.Instance.SetPlayableChar(
                m_playerId.Value,
                m_charSelected.Value,
                IsOwner);
        }
        // Assign the name of the object base on the player id on every instance
        gameObject.name = PlayerData.Instance.playerName;
        
        Button leftBtn = leftButton.GetComponent<Button>();
        Button rightBtn = rightButton.GetComponent<Button>();
        Button readyBtn = readyButton.GetComponent<Button>();
        Button cancleBtn = cancleButton.GetComponent<Button>();
        
        leftBtn.onClick.AddListener(LeftButtonOnClick);
        rightBtn.onClick.AddListener(RightButtonOnClick);
        readyBtn.onClick.AddListener(ReadyButtonOnClick);
        cancleBtn.onClick.AddListener(CancleButtonOnClick);
    }
    
    private void OnPlayerIdSet(int oldValue, int newValue)
    {
        CharacterSelectionManager.Instance.SetPlayableChar(newValue, newValue, IsOwner);

        if (IsServer)
            m_charSelected.Value = newValue;
    }

    // Event call when server changes the network variable
    private void OnCharacterChanged(int oldValue, int newValue)
    {
        // If I am not the owner, update the character selection UI
        if (!IsOwner && HasSelectCharacter())
            CharacterSelectionManager.Instance.SetCharacterUI(m_playerId.Value, newValue);
    }
    IEnumerator HostShutdown()
    {
        // Tell the clients to shutdown
        ShutdownClientRpc();

        // Wait some time for the message to get to clients
        yield return new WaitForSeconds(0.5f);

        // Shutdown server/host
        Shutdown();
    }

    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        LoadingSceneManager.Instance.LoadScene(SceneName.MainMenu, false);
    }

    [ClientRpc]
    void ShutdownClientRpc()
    {
        if (IsServer)
            return;

        Shutdown();
    }
    private void ChangeCharacterSelection(int value)
    {
        // Assign a temp value to prevent the call of onchange event in the charSelected
        int charTemp = m_charSelected.Value;
        charTemp += value;

        if (charTemp >= CharacterSelectionManager.Instance.charactersData.Length)
            charTemp = 0;
        else if (charTemp < 0)
            charTemp = CharacterSelectionManager.Instance.charactersData.Length - 1;

        if (IsOwner)
        {
            // Notify server of the change
            ChangeCharacterSelectionServerRpc(charTemp);

            // Owner doesn't wait for the onvaluechange
            CharacterSelectionManager.Instance.SetPlayableChar(
                m_playerId.Value,
                charTemp,
                IsOwner);
        }
    }

    [ServerRpc]
    private void ChangeCharacterSelectionServerRpc(int newValue)
    {
        m_charSelected.Value = newValue;
    }

    [ServerRpc]
    private void ReadyServerRpc()
    {
        CharacterSelectionManager.Instance.PlayerReady(
            OwnerClientId,
            m_playerId.Value,
            m_charSelected.Value,
            PlayerData.Instance.playerName
            );
    }

    [ServerRpc]
    private void NotReadyServerRpc()
    {
        CharacterSelectionManager.Instance.PlayerNotReady(OwnerClientId, m_charSelected.Value);
    }

    // The arrows on the player are not meant to works as buttons
    private void OnUIButtonPress(ButtonActions buttonAction)
    {
        if (!IsOwner)
            return;

        switch (buttonAction)
        {
            case ButtonActions.lobby_ready:
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                    true,
                    m_charSelected.Value);

                ReadyServerRpc();
                break;

            case ButtonActions.lobby_not_ready:
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                    false,
                    m_charSelected.Value);

                NotReadyServerRpc();
                break;
        }
    }

    private void Update()
    {
        if (PlayerData.Instance.lobbyCode == null)
        {
            roomCode.text = "Room Code : " + PlayerData.Instance.joinCode;
        }
        else
        {
            roomCode.text = "Room Code : " + PlayerData.Instance.lobbyCode;
        }
        
        if (IsOwner && CharacterSelectionManager.Instance.GetConnectionState(m_playerId.Value) != ConnectionState.ready)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeCharacterSelection(-1);
                
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ChangeCharacterSelection(1);
                
            }
        }

        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {

                // Check that the character is not selected
                if (!CharacterSelectionManager.Instance.IsReady(m_charSelected.Value))
                {
                    CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                        true,
                        m_charSelected.Value);

                    ReadyServerRpc();
                }
                else
                {
                    // if selected check if is selected by me
                    if (CharacterSelectionManager.Instance.IsSelectedByPlayer(
                            m_playerId.Value, m_charSelected.Value))
                    {
                        // If it's selected by me, de-select
                        CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                            false,
                            m_charSelected.Value);

                        NotReadyServerRpc();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // exit the network state and return to the menu
                if (m_playerId.Value == 0) // Host
                {
                    // All player should shutdown and exit
                    StartCoroutine(HostShutdown());
                }
                else
                {
                    Shutdown();
                }
            }
        }
    }
    private void OnEnable()
    {
        m_playerId.OnValueChanged += OnPlayerIdSet;
        m_charSelected.OnValueChanged += OnCharacterChanged;
        //OnButtonPress.a_OnButtonPress += OnUIButtonPress;
    }

    private void OnDisable()
    {
        m_playerId.OnValueChanged -= OnPlayerIdSet;
        m_charSelected.OnValueChanged -= OnCharacterChanged;
        //OnButtonPress.a_OnButtonPress -= OnUIButtonPress;
    }
    public void Despawn()
    {
        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }
    
    public void LeftButtonOnClick()
    {
        ChangeCharacterSelection(-1);
        //check the button is clicked
        Debug.Log("You have clicked the LEFT button!");
    }
    
    public void RightButtonOnClick()
    {
        ChangeCharacterSelection(1);
        //check the button is clicked
        Debug.Log("You have clicked the Right button!");
    }
    
    public void ReadyButtonOnClick()
    {
        //check the button is clicked
        Debug.Log("You have clicked the Ready button!");
        
        readyUI.SetActive(false);
        cancleUI.SetActive(true);
        
        ReadyServerRpc();
    }
    
    public void CancleButtonOnClick()
    {
        //check the button is clicked
        Debug.Log("You have clicked the Cancle button!");
        
        readyUI.SetActive(true);
        cancleUI.SetActive(false);
        
        NotReadyServerRpc();
    }
}
