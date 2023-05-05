using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonProtocol : NetworkBehaviour
{
    public static ButtonProtocol Instance;

    public Button LeftButton => _leftButton;
    public Button RightButton => _rightButton;
    
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    private void Awake()
    {
        Instance = this;
    }
}
