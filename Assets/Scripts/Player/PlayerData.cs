using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{
    public FixedString32Bytes playerName;
    public string lobbyCode;
    public string joinCode;

    //public CharacterData characterData;
}
