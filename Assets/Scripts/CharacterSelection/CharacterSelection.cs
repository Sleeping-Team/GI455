using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int indexCharacter;
    public GameObject lightSelected;

    [SerializeField] private TMP_Text roomCode;
    
    void Update()
    {
        roomCode.text = PlayerData.Instance.lobbyCode;
    }
    
    public void SetNonPlayableChar()
    {
        foreach (var character in characters)
        {
            character.SetActive(false);
        }
    }

    public void SetPlayableChar(int index)
    {
        SetNonPlayableChar();
        
        characters[index].SetActive(true);
    }

    public void ShowLight(bool isLightOn)
    {
        lightSelected.SetActive(isLightOn);
    }
}
