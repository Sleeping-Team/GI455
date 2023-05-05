using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplay : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;

    private void Start()
    {
        SetName();
    }

    private void SetName()
    {
        if (IsHost || IsClient)
        {
            _name.text = PlayerData.Instance.playerName.ToString();
        }

        if (PlayerData.Instance.playerName == null)
        {
            _name.text = "Name";
        }
    }
}
