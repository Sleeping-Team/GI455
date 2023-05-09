using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ProfileHolder : NetworkBehaviour
{
    public MenuProperties MenuMenuProfile => _menuProfile;
    [SerializeField] private MenuProperties _menuProfile;

    public override void OnNetworkSpawn()
    {
        name = _menuProfile.name;
        
        base.OnNetworkSpawn();
    }
}
