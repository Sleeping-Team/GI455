using UnityEngine;
using Unity.Netcode;

public class Character : NetworkBehaviour
{
    public TablePosition Table => _table;
    
    public bool IsWalk
    {
        get => _isWalk;
        set => _isWalk = value;
    }

    public bool IsSit
    {
        get => _isSit;
        set => _isSit = value;
    }
    
    protected bool _isWalk;
    protected bool _isSit;
    
    protected TablePosition _table;
}
