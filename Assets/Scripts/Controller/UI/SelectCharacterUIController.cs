using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCharacterUIController : MonoBehaviour
{
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button selectButton;

    [SerializeField] public CharacterData[] _characterDatas;
    [SerializeField] public Image _characterImage;
    [SerializeField] public TMP_Text _characterName;

    private int i = 0;
    private int a = 1;
    private int b = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        Button rightBtn = rightButton.GetComponent<Button>();
        Button leftBtn = leftButton.GetComponent<Button>();
        Button selectBtn = selectButton.GetComponent<Button>();

        rightBtn.onClick.AddListener(RightButtonOnClick);
        leftBtn.onClick.AddListener(LeftButtonOnClick);
        selectBtn.onClick.AddListener(SelectButtonOnClick);

        _characterImage.sprite = _characterDatas[i].characterProfile;
        _characterName.text = _characterDatas[i].characterName;
    }

    private void Update()
    {
        //display CharacterData
        _characterImage.sprite = _characterDatas[i].characterProfile;
        _characterName.text = _characterDatas[i].characterName;
    }

    void RightButtonOnClick()
    {
        Debug.Log ("Right " + a);

        a = a + 1;
        
        //keep rollingggg right
        if (i == 3)
        { 
            i = 0;
        }
        if (i >= 0)
        { 
            i = i + 1;  
        }
        
        
    }
    
    void LeftButtonOnClick()
    {
        Debug.Log ("Left " + b);

        b = b + 1;

        //keep rollingggg left
        if (i == 0)
        {
            i = 3;
        }
        if (i <= 3)
        {
            i = i - 1;
        }
        
        
    }
    
    void SelectButtonOnClick()
    {
        Debug.Log ("Select");

        //keep the data in PlayerData
        PlayerData.Instance.characterData = _characterDatas[i];

        SceneManager.LoadScene("Gameplay Lab");
    }
    
}
