using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    private string playerInputName;
    
    [SerializeField] private Button confirmButton;

    [SerializeField] private TMP_InputField nameInput;
    //[SerializeField] private TMP_Text playerName;

    private void Start()
    {
        Button confirmBtn = confirmButton.GetComponent<Button>();
        
        confirmBtn.onClick.AddListener(ConfirmButtonOnClick);
    }

    public void StoreName()
    {
        //get player name from Inputfield
        playerInputName = nameInput.GetComponent<TMP_InputField>().text;
        //keep name in player data
        PlayerData.Instance.playerName = playerInputName;
        
        Debug.Log("Get the name. " + playerInputName);
        //temporary test
        //ShowName();
    }

    public void ConfirmButtonOnClick()
    {
       SceneManager.LoadScene("MainMenu"); 
       //SceneManagement.Instance.LoadScene(SceneName.MainMenu,false);
    }
    
    /*
    public void ShowName()
    {
        //show name as text
        playerName.GetComponent<TMP_Text>().text = playerInputName;
    }*/
}
