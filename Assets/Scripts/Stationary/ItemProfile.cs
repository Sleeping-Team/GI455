using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Profile/Item")]
public class ItemProfile : ScriptableObject
{
    [SerializeField] private ItemProperties _items;

    public ItemProperties items => _items;
}
