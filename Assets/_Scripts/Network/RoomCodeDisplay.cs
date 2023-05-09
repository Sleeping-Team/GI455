using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RoomCodeDisplay : NetworkBehaviour
{
    [SerializeField]private TMP_Text roomCode;
    
    public override void OnNetworkSpawn()
    {
        if(IsHost) roomCode.text = "Room Code : " + PlayerData.Instance.lobbyCode;
        else if(IsClient && !IsHost) roomCode.text = "Room Code : " + PlayerData.Instance.joinCode;
        
        base.OnNetworkSpawn();
    }
}
