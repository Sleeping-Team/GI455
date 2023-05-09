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
    
    [SerializeField] private Button clickButton;
    [SerializeField] private Button confirmButton;

    [Space] 
    [SerializeField] private GameObject clickToStartUI;
    [SerializeField] private GameObject getNameUI;
    
    [Space]
    
    [SerializeField] private TMP_InputField nameInput;
    //[SerializeField] private TMP_Text playerName;

    private void Start()
    {
        Button clickbtn = clickButton.GetComponent<Button>();
        Button confirmBtn = confirmButton.GetComponent<Button>();
        
        clickbtn.onClick.AddListener(ClickToStart);
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

    public void ClickToStart()
    {
        clickToStartUI.SetActive(false);
        getNameUI.SetActive(true);
    }
    public void ConfirmButtonOnClick()
    {
       LoadingSceneManager.Instance.LoadScene(SceneName.MainMenu); 
       //SceneManagement.Instance.LoadScene(SceneName.MainMenu,false);
    }

    /*
    public void ShowName()
    {
        //show name as text
        playerName.GetComponent<TMP_Text>().text = playerInputName;
    }*/
}
