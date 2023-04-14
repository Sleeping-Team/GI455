using UnityEngine;

[CreateAssetMenu (menuName ="Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Data")]
    public string characterName;
    public GameObject characterPrefab;
    public Sprite characterProfile;

    [Header("Client Info")]
    public ulong clientId;
    public int playerId;
    public bool isSelected;

    void OnEnable()
    {
        EmptyData();
    }

    public void EmptyData()
    {
        isSelected = false;
        clientId = 0;
        playerId = -1;
    }
}
