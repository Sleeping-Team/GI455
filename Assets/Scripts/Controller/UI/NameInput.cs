using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    private string playerInputName;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text playerName;

    public void StoreName()
    {
        //get player name from Inputfield
        playerInputName = nameInput.GetComponent<TMP_InputField>().text;
        Debug.Log("Get the name. " + playerInputName);
        //temporary test
        ShowName();
    }

    public void ShowName()
    {
        //show name as text
        playerName.GetComponent<TMP_Text>().text = playerInputName;
    }
}
