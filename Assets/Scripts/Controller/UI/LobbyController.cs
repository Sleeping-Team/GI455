using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyCode;
    
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.Instance.lobbyCode == null)
        {
            _lobbyCode.text = PlayerData.Instance.joinCode;
        }
        else
        {
            _lobbyCode.text = PlayerData.Instance.lobbyCode;
        }

        Button continueBtn = continueButton.GetComponent<Button>();
        Button exitBtn = exitButton.GetComponent<Button>();
        
        continueBtn.onClick.AddListener(ContinueButtonOnClick);
        exitBtn.onClick.AddListener(ExitButtonOnClick);
        
        //pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lobbyCode.text == null)
        {
            if (PlayerData.Instance.lobbyCode == null)
            {
                _lobbyCode.text = PlayerData.Instance.joinCode;
            }
            else if(PlayerData.Instance.joinCode == null)
            {
                _lobbyCode.text = PlayerData.Instance.lobbyCode;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
    }
    
    public void ContinueButtonOnClick()
    {
        //check the button is clicked
        Debug.Log("You have clicked the continue button!");
        
        pauseUI.SetActive(false);
    }
    
    public void ExitButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the exit button!");

        SceneManager.LoadScene("MainMenu");
    }
}
