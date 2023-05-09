using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Profile/Menu")]
public class MenuProperties : ScriptableObject
{
    [SerializeField] private string itemName;
    [FormerlySerializedAs("itemModel")] [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite itemImage;

    public string name => itemName;
    public GameObject prefab => itemPrefab;
    public Sprite image => itemImage;
}
