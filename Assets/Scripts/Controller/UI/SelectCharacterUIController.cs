using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterUIController : MonoBehaviour
{
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button selectButton;
    
    // Start is called before the first frame update
    void Start()
    {
        Button rightBtn = rightButton.GetComponent<Button>();
        Button leftBtn = leftButton.GetComponent<Button>();
        Button selectBtn = selectButton.GetComponent<Button>();

        rightBtn.onClick.AddListener(RightButtonOnClick);
        leftBtn.onClick.AddListener(LeftButtonOnClick);
        selectBtn.onClick.AddListener(SelectButtonOnClick);
    }

    void RightButtonOnClick()
    {
        Debug.Log ("Right");
    }
    
    void LeftButtonOnClick()
    {
        Debug.Log ("Left");
    }
    
    void SelectButtonOnClick()
    {
        Debug.Log ("Select");
    }
}
