using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemProperties
{
    [SerializeField] private string itemName;
    [SerializeField] private GameObject itemModel;
    [SerializeField] private Sprite itemImage;

    public string name => itemName;
    public GameObject model => itemModel;
    public Sprite image => itemImage;
}
