using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public Image[] characterProfiles;
    public int indexCharacter;
    public GameObject lightSelected;
    
    void Update()
    {
        
    }
    
    public void SetNonPlayableChar()
    {
        for (int i = 0; i < characters.Length ; i++)
        {
            characterProfiles[i].enabled = false;
            characters[i].SetActive(false);
            lightSelected.SetActive(false);
        }
    }

    public void SetPlayableChar(int index)
    {
        SetNonPlayableChar();
        
        characters[index].SetActive(true);
        characterProfiles[index].enabled = true;
    }

    public void ShowLight(bool isLightOn)
    {
        lightSelected.SetActive(isLightOn);
    }
}
