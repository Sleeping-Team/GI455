using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class MenuController : MonoBehaviour
{
    //Button
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button confirmButton;
    
    [Space]
    
    [SerializeField] private TMP_InputField _joinInput;
    
    [Space]
    
    [SerializeField] private TMP_Text pName;

    [Space]

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject joinUI;
    
    [Space]
    
    [SerializeField] private SceneName nextScene = SceneName.SelectCharacter;
    
    [Space]
    
    [SerializeField]
    private CharacterData[] m_characterDatas;
    [SerializeField]
    private LobbyData m_lobbyData;

    private Lobby _connnectedLobby;
    private UnityTransport _transport;
    private string _playerId;
    private string joinInput;
    
    private void Awake() => _transport = FindObjectOfType<UnityTransport>();
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        ClearAllCharacterData();

        pName.GetComponent<TMP_Text>().text = PlayerData.Instance.playerName.ToString();
        
        Button createBtn = createButton.GetComponent<Button>();
        Button joinBtn = joinButton.GetComponent<Button>();
        Button backBtn = backButton.GetComponent<Button>();
        Button exitBtn = exitButton.GetComponent<Button>();
        Button confirmBtn = confirmButton.GetComponent<Button>();

        createBtn.onClick.AddListener(CreateButtonOnClick);
        joinBtn.onClick.AddListener(JoinButtonOnClick);
        backBtn.onClick.AddListener(BackButtonOnClick);
        exitBtn.onClick.AddListener(ExitButtonOnClick);

        confirmBtn.onClick.AddListener(ConfirmButtonOnClick);
        
        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);
        
        LoadingSceneManager.Instance.Init();
    }
    
    void CreateButtonOnClick()
    {
        Debug.Log ("You have clicked the create button!");
        
        CreateOrJoinLobby();
        CreateLobby();
    }

    void JoinButtonOnClick()
    {
        //check the button is clicked
        Debug.Log ("You have clicked the join button!");
        
        menuUI.SetActive(false);
        joinUI.SetActive(true);
        
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
        Debug.Log("You have clicked the confirm button!");
        
        OnClickJoin();
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

    private void ClearAllCharacterData()
    {
        foreach (CharacterData data in m_characterDatas)
        {
            data.EmptyData();
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
        }
        catch (Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }

    public async void CreateOrJoinLobby()
    {
        await Authenticate();
    }

    private async Task Authenticate()
    {
        var option = new InitializationOptions();
        
        #if UNITY_EDITOR
        option.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
        #endif

        await UnityServices.InitializeAsync(option);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
        m_lobbyData.playerId = _playerId;
    }

    private async Task CreateLobby()
    {
        const int maxPlayer = 4;

        var a = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        PlayerData.Instance.lobbyCode = joinCode;
            
        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        OnClickHost();
            
    }
    
    private void SetTransformAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }
    public void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();
        
        LoadingSceneManager.Instance.LoadScene(nextScene);
    }

    public async void OnClickJoin()
    {
        JoinAllocation a = null;
        if (PlayerData.Instance.joinCode != null)
        {
            a = await RelayService.Instance.JoinAllocationAsync
                (PlayerData.Instance.joinCode);
            
            SetTransformAsClient(a);
            StartCoroutine(Join());

            PlayerData.Instance.lobbyCode = PlayerData.Instance.joinCode;
        }
        else
        {
            Debug.LogError("Can't join lobby");
        }
    }

    private IEnumerator Join()
    {
        LoadingFadeEffect.Instance.FadeAll();

        yield return new WaitUntil(() => LoadingFadeEffect.s_canLoad);

        NetworkManager.Singleton.StartClient();
    }
    
}
