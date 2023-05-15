using UnityEngine;
using Unity.Netcode;
using FlatKit;
using UnityEngine.UI;
using DG.Tweening;

public class Grabbable : NetworkBehaviour, IInteractable
{
    //[SerializeField] private float _grabDistance = 5.0f;

    private Rigidbody m_Rigidbody;
    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();

    private MeshRenderer[] _meshRenderers;
    private MeshRenderer _thisMeshRenderer;
    
    private Canvas _interactingCanvas;
    private bool _iconIsDisabled = false;

    public enum Selection
    {
        Selected,
        Deselected,
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        gameObject.TryGetComponent(out _thisMeshRenderer);
        
        _meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
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

    public void SelectionState(Selection state)
    {
        switch (state)
        {
            case Selection.Selected:
                if (_thisMeshRenderer != null)
                {
                    foreach (Material mat in _thisMeshRenderer.materials)
                    {
                        mat.SetFloat("_OutlineEnabled", 1);
                    }
                }
                
                foreach (MeshRenderer renderer in _meshRenderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        mat.SetFloat("_OutlineEnabled", 1);
                    }
                }
                break;
            case Selection.Deselected:
                if (_thisMeshRenderer != null)
                {
                    foreach (Material mat in _thisMeshRenderer.materials)
                    {
                        mat.SetInt("_OutlineEnabled", 0);
                    }
                }
                
                foreach (MeshRenderer renderer in _meshRenderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        mat.SetInt("_OutlineEnabled", 0);
                    }
                }
                break;
        }
    }

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
            
            Destroy(gameObject);
        }
    }

    public void PlaceOnTable(string tableName)
    {
        ReleaseServerRpc();

        PlaceServerRpc(tableName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceServerRpc(string name)
    {
        transform.parent = GameObject.Find(name).transform;
        transform.position = Vector3.zero;
        gameObject.tag = "Disable";
    }

    public void OnEnter()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveY(-243.788f, 1f);
        interactionIcon.DOFade(1f, 1f);
    }

    public void OnExit()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveY(-243.905f, 1f);
        interactionIcon.DOFade(0f, 0.5f);
    }

    public void DisableIcon()
    {
        _iconIsDisabled = true;
        _interactingCanvas.gameObject.SetActive(false);
    }

    public void EnableIcon()
    {
        _iconIsDisabled = false;
        _interactingCanvas.gameObject.SetActive(true);
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
