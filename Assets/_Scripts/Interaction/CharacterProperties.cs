using UnityEngine;
using Unity.Netcode;

public abstract class CharacterProperties : NetworkBehaviour
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

    public bool HaveWait
    {
        get => _haveWait;
        set => _haveWait = value;
    }
    protected bool _isWalk;
    protected bool _isSit;
    protected bool _haveWait;
    
    protected TablePosition _table;
}
