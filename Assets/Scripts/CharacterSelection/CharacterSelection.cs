using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int indexCharacter;
    public GameObject isSelected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNonPlayableChar()
    {
        foreach (var character in characters)
        {
            character.SetActive(false);
            Debug.Log("Is not active");
        }
    }

    public void SetPlayableChar(int index)
    {
        SetNonPlayableChar();
        characters[index].SetActive(true);
    }
    
    public void ShowLight(bool isLightOn)
    {
        isSelected.SetActive(isLightOn);
    }
}
