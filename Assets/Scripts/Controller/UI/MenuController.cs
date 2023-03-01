using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text pName;
    
    // Start is called before the first frame update
    void Start()
    {
        //show name as text
        pName.GetComponent<TMP_Text>().text = PlayerData.Instance.playerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
