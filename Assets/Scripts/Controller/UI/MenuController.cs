using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //Button
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button confirmButton;
    
    
    [SerializeField] private TMP_InputField _joinInput;
    
    [SerializeField] private TMP_Text pName;
    [SerializeField] private TMP_Text _joinCodeText;

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject joinUI;

    private string joinInput;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //show name as text
        pName.GetComponent<TMP_Text>().text = PlayerData.Instance.playerName;
        
        Button createBtn =createButton.GetComponent<Button>();
        Button joinBtn = joinButton.GetComponent<Button>();
        Button backBtn = backButton.GetComponent<Button>();
        Button exitBtn = exitButton.GetComponent<Button>();
        Button confirmBtn = confirmButton.GetComponent<Button>();

        createBtn.onClick.AddListener(CreateButtonOnClick);
        joinBtn.onClick.AddListener(JoinButtonOnClick);
        backBtn.onClick.AddListener(BackButtonOnClick);
        exitBtn.onClick.AddListener(ExitButtonOnClick);

        confirmBtn.onClick.AddListener(ConfirmButtonOnClick);

        
        RelayController.Instance.Initialize();
    }

    private void Update()
    {
        if (_joinCodeText.text == null)
        {
            _joinCodeText.text = PlayerData.Instance.lobbyCode;
        }
            
    }

    void CreateButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the create button!");
        
        RelayController.Instance.CreateGame();
        _joinCodeText.text = PlayerData.Instance.lobbyCode;

        //SceneManager.LoadScene("Gameplay Lab");
    }

    void JoinButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the join button!");
        
        menuUI.SetActive(false);
        joinUI.SetActive(true);
        /*
        _joinInput.text = _joinInput.text.ToUpper();

        PlayerData.Instance.joinCode = _joinCodeText.text;
        */
    }

    public void StoreCode()
    {
        //get player name from Inputfield
        joinInput = _joinInput.GetComponent<TMP_InputField>().text;
        joinInput = joinInput.ToUpper();
        //keep name in player data
        PlayerData.Instance.joinCode = joinInput;

        Debug.Log("Get the code. " + joinInput);
        //temporary test
        //ShowName();
    }

    void BackButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the back button!");
        
        joinUI.SetActive(false);
        menuUI.SetActive(true);
    }
    

    public void ConfirmButtonOnClick()
    {
        //check the button is clicked
        Debug.Log("You have clicked the confirm button!");

        RelayController.Instance.JoinGame(joinInput);
        //SceneManager.LoadScene("Gameplay Lab");
    }

    void ExitButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the exit button!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
