using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class RelayController : Singleton<RelayController>
{
    private UnityTransport _transport;

    private const int MaxPlayers = 4;

    private async void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();
        
        //_button.SetActive(false);
        await Authenticate();
        //_button.SetActive(true);
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        //_button.SetActive(false);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        
        PlayerData.Instance.lobbyCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        
        _transport.SetHostRelayData(a.RelayServer.IpV4,
            (ushort)a.RelayServer.Port,a.AllocationIdBytes,a.Key,a.ConnectionData);

        NetworkManager.Singleton.StartHost();
        
    }
    
    public async void JoinGame(string ID)
    {
        //_button.SetActive(false);

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(ID);
        _transport.SetClientRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port,a.AllocationIdBytes,a.Key,a.ConnectionData,a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

    public async Task Initialize()
    { 
        }
}
