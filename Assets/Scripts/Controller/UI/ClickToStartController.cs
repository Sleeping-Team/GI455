using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToStartController : MonoBehaviour
{
	[SerializeField] private Button closeButton;
	[SerializeField] private GameObject firstScene;
	[SerializeField] private GameObject nameScene;
	
	void Start () 
	{
		Button btn = closeButton.GetComponent<Button>();
		btn.onClick.AddListener(ButtonOnClick);
	}

	void ButtonOnClick()
	{
		Debug.Log ("Get Start!");
		firstScene.SetActive(false);
		nameScene.SetActive(true);
	}
    
}
