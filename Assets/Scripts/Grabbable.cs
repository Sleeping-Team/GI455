using UnityEngine;
using Unity.Netcode;

public class Grabbable : NetworkBehaviour
{
    //[SerializeField] private float _grabDistance = 5.0f;

    private Rigidbody m_Rigidbody;
    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (!NetworkObject.IsSpawned)
        {
            NetworkObject.Spawn();
        }

        base.OnNetworkSpawn();
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
    //                 Debug.Log("In Range");
    //                 if (Input.GetKeyDown(KeyCode.E))
    //                 {
    //                     Debug.Log("Press E");
    //                     TryGrabServerRpc();
    //                 }
    //             }
    //         }
    //     }
    // }

    public void CarryLogic()
    {
        var localPlayerObject = NetworkManager?.SpawnManager?.GetLocalPlayerObject();
        
        if (m_IsGrabbed.Value)
        {
            if (IsOwner) ReleaseServerRpc();
        }
        else
        {
            if (localPlayerObject != null)
            {
                TryGrabServerRpc();
            }
        }
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (parentNetworkObject != null && (IsOwner || IsServer))
        {
            transform.localPosition = Vector3.up * 2;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryGrabServerRpc(ServerRpcParams serverRpcParams = default)
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
    private void ReleaseServerRpc()
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

    public void PlaceOnTable(Transform table)
    {
        ReleaseServerRpc();

        transform.parent = table;
        transform.position = Vector3.zero;
        gameObject.tag = "Disable";
    }
}
