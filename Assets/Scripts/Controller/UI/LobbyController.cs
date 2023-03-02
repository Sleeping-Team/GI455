using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyCode;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
            if (PlayerData.Instance.joinCode == null)
            {
                _lobbyCode.text = PlayerData.Instance.lobbyCode;
            }
            
        }
    }
}
