using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharSelection : NetworkBehaviour
{
    private const int k_noCharacterSelectedValue = -1;

    [SerializeField] private NetworkVariable<int> m_charSelected =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

    [SerializeField] private NetworkVariable<int> m_playerId =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

    private NetworkVariable<FixedString32Bytes> m_playerName =
        new NetworkVariable<FixedString32Bytes>("waiting");

    public int CharSelected => m_charSelected.Value;

    private bool HasAcharacterSelected()
    {
        return m_playerId.Value != k_noCharacterSelectedValue;
    }

    private void Start()
    {
        if (IsServer)
        {
            m_playerId.Value = CharacterSelectionManager.Instance.GetPlayerId(OwnerClientId);
        }
        else if(!IsOwner && HasAcharacterSelected())
        {
            ChangeNameServerRpc(PlayerData.Instance.playerName);
            CharacterSelectionManager.Instance.SetPlayebleChar(m_playerId.Value,m_charSelected.Value,IsOwner,m_playerName.Value.ToString());
        }

        gameObject.name = $"Player{m_playerId.Value + 1}";
        
        ButtonProtocol.Instance.LeftButton.onClick.AddListener(OnClickLeft);
        ButtonProtocol.Instance.RightButton.onClick.AddListener(OnClickRight);
    }

    private void OnPlayerIdSet(int oldValue, int newValue)
    {
        ChangeNameServerRpc(PlayerData.Instance.playerName);
        CharacterSelectionManager.Instance.SetPlayebleChar(newValue,newValue,IsOwner,m_playerName.Value.ToString());

        if (IsServer)
        {
            m_charSelected.Value = newValue;
        }
    }

    private void OnCharacterChanged(int oldValue, int newValue)
    {
        if (!IsOwner && HasAcharacterSelected())
        {
            CharacterSelectionManager.Instance.SetCharacterUI(m_playerId.Value,newValue);
        }
    }

    IEnumerator HostShutdown()
    {
        ShutdownClientRpc();

        yield return new WaitForSeconds(0.5f);

        Shutdown();
    }

    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        LoadingSceneManager.Instance.LoadScene(SceneName.MainMenu,false);
    }

    [ClientRpc]
    void ShutdownClientRpc()
    {
        if (IsServer)
        {
            return;
        }
        
        Shutdown();
    }

    private void ChangeCharacterSelection(int value)
    {
        int charTemp = m_charSelected.Value;
        charTemp += value;

        if (charTemp >= CharacterSelectionManager.Instance.charactersData.Length)
        {
            charTemp = 0;
        }
        else if (charTemp < 0)
        {
            charTemp = CharacterSelectionManager.Instance.charactersData.Length - 1;
        }

        if (IsOwner)
        {
            ChangeCharacterSelectionServerRpc(charTemp);
            
            CharacterSelectionManager.Instance.SetPlayebleChar(m_playerId.Value,charTemp,IsOwner,m_playerName.Value.ToString());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCharacterSelectionServerRpc(int newValue)
    {
        m_charSelected.Value = newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeNameServerRpc(FixedString32Bytes newName)
    {
        m_playerName.Value = newName;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyServerRpc()
    {
        CharacterSelectionManager.Instance.PlayerReady(OwnerClientId,m_playerId.Value,m_charSelected.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotReadyServerRpc()
    {
        CharacterSelectionManager.Instance.PlayerNotReady(OwnerClientId,m_charSelected.Value);
    }

    private void OnUIButtonPress(ButtonActions buttonAction)
    {
        if (!IsOwner)
        {
            return;
        }

        switch (buttonAction)
        {
            case ButtonActions.lobby_ready:
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(true,m_charSelected.Value);
                
                ReadyServerRpc();
                break;
            
            case ButtonActions.lobby_not_ready:
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(false,m_charSelected.Value);
                
                NotReadyServerRpc();
                break;
        }
    }

    private void Update()
    {
        if (IsOwner && CharacterSelectionManager.Instance.GetConnectionState(m_playerId.Value) != ConnectionState.ready)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnClickLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                OnClickRight();
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
        }

    }
    
    private void OnEnable()
    {
        m_playerId.OnValueChanged += OnPlayerIdSet;
        m_charSelected.OnValueChanged += OnCharacterChanged;
        OnButtonPress.a_OnButtonPress += OnUIButtonPress;
    }

    private void OnDisable()
    {
        m_playerId.OnValueChanged -= OnPlayerIdSet;
        m_charSelected.OnValueChanged -= OnCharacterChanged;
        OnButtonPress.a_OnButtonPress -= OnUIButtonPress;
    }
    public void Despawn()
    {
        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }

    public void OnClickLeft()
    {
        ChangeCharacterSelection(-1);
    }

    public void OnClickRight()
    {
        ChangeCharacterSelection(1);
    }

    public void OnClickReady()
    {
        if (!CharacterSelectionManager.Instance.IsReady(m_charSelected.Value))
            {
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(true,m_charSelected.Value);
                
                ReadyServerRpc();
            }
            else
            {
                Debug.Log("It's Bug");
            }
    }

    public void OnClickCancle()
    {
        if (CharacterSelectionManager.Instance.IsSelectedByPlayer(m_playerId.Value,m_charSelected.Value))
        {
            CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(false,m_charSelected.Value);
                    
            NotReadyServerRpc();
        }
    }

    public void OnClickBack()
    {
        if (m_playerId.Value == 0)
        {
            StartCoroutine(HostShutdown());
        }
        else
        {
            Shutdown();
        }
    }
}
