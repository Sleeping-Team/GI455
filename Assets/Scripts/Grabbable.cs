using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Grabbable : NetworkBehaviour
{
    //[SerializeField] private float _grabDistance = 5.0f;

    [SerializeField] private Rigidbody m_Rigidbody;

    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();

    private void Awake()
    {
        if(m_Rigidbody == null) m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (NetworkManager == null)
        {
            return;
        }

        if (m_Rigidbody)
        {
            m_Rigidbody.isKinematic = !IsServer || m_IsGrabbed.Value;
        }
    }

    // private void Update()
    // {
    //     if (NetworkManager == null)
    //     {
    //         return;
    //     }
    //
    //     var localPlayerObject = NetworkManager?.SpawnManager?.GetLocalPlayerObject();
    //
    //     if (m_IsGrabbed.Value)
    //     {
    //         if (IsOwner && Input.GetKeyDown(KeyCode.E))
    //         {
    //             ReleaseServerRpc();
    //         }
    //     }
    //     else
    //     {
    //         if (localPlayerObject != null)
    //         {
    //             var distance = Vector3.Distance(transform.position, localPlayerObject.transform.position);
    //             if (distance <= _grabDistance)
    //             {
    //                 if (Input.GetKeyDown(KeyCode.E))
    //                 {
    //                     TryGrabServerRpc();
    //                 }
    //             }
    //         }
    //     }
    // }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (parentNetworkObject != null && (IsOwner || IsServer))
        {
            transform.localPosition = Vector3.up * 2;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryGrabServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Try Grab Server Rpc");
        if (!m_IsGrabbed.Value)
        {
            Debug.Log("Grabbable");
            
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var senderPlayerObject = PlayerMovement.Players[senderClientId].NetworkObject;
            if (senderPlayerObject != null)
            {
                Debug.Log("Transfer Ownership");
                NetworkObject.ChangeOwnership(senderClientId);

                transform.parent = senderPlayerObject.transform;

                m_IsGrabbed.Value = true;
            }
        }
    }

    [ServerRpc]
    public void ReleaseServerRpc()
    {
        Debug.Log("Try Release Server Rpc");
        if (m_IsGrabbed.Value)
        {
            Debug.Log("Remove Ownership");
            NetworkObject.RemoveOwnership();

            transform.parent = null;

            m_IsGrabbed.Value = false;
        }
    }
}
