using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharSelection : NetworkBehaviour
{
    private const int k_noCharacterSelectedValue = -1;

    [SerializeField] private NetworkVariable<int> m_charSelected =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

    [SerializeField] private NetworkVariable<int> m_playerId =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

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
            CharacterSelectionManager.Instance.SetPlayebleChar(m_playerId.Value,m_charSelected.Value,IsOwner);
        }

        gameObject.name = $"Player{m_playerId.Value + 1}";
    }

    private void OnPlayerIdSet(int oldValue, int newValue)
    {
        CharacterSelectionManager.Instance.SetPlayebleChar(newValue,newValue,IsOwner);

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
            
            CharacterSelectionManager.Instance.SetPlayebleChar(m_playerId.Value,charTemp,IsOwner);
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
        CharacterSelectionManager.Instance.PlayerReady(OwnerClientId,m_playerId.Value,m_charSelected.Value);
    }

    [ServerRpc]
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
                ChangeCharacterSelection(-1);
                
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ChangeCharacterSelection(1);
                
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
}
