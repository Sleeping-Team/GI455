using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] private ItemProfile profile;

    private ItemProperties _itemOnPlace;
    
    enum Type
    {
        Food,
        Drink
    }
}
