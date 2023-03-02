using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //Button
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;
    
    
    [SerializeField] private TMP_InputField _joinInput;
    
    [SerializeField] private TMP_Text pName;
    [SerializeField] private TMP_Text _joinCodeText;

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject joinUI;
    
    
    private UnityTransport _transport;

    private const int MaxPlayers = 4;
    
    // Start is called before the first frame update
    void Start()
    {
        //show name as text
        pName.GetComponent<TMP_Text>().text = PlayerData.Instance.playerName;
        
        Button createBtn =createButton.GetComponent<Button>();
        Button joinBtn = joinButton.GetComponent<Button>();
        Button backBtn = backButton.GetComponent<Button>();
        Button exitBtn = exitButton.GetComponent<Button>();

        createBtn.onClick.AddListener(CreateButtonOnClick);
        joinBtn.onClick.AddListener(JoinButtonOnClick);
        backBtn.onClick.AddListener(BackButtonOnClick);
        exitBtn.onClick.AddListener(ExitButtonOnClick);
    }

    void CreateButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the create button!");
        
        CreateGame();
    }

    void JoinButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the join button!");
        
        menuUI.SetActive(false);
        joinUI.SetActive(true);
        
        JoinGame();
    }
    
    void BackButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the back button!");
        
        joinUI.SetActive(false);
        menuUI.SetActive(true);
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
    
    public async void CreateGame()
    {
        //_button.SetActive(false);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        
        _joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        
        _transport.SetHostRelayData(a.RelayServer.IpV4,
            (ushort)a.RelayServer.Port,a.AllocationIdBytes,a.Key,a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }
    
    public async void JoinGame()
    {
        //_button.SetActive(false);

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(_joinInput.text);
        _transport.SetClientRelayData(a.RelayServer.IpV4,(ushort)a.RelayServer.Port,a.AllocationIdBytes,a.Key,a.ConnectionData,a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}
