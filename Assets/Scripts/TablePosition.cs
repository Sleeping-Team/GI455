using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePosition : MonoBehaviour
{
    public Transform[] ChairPosition => _chairPosition;
    public Transform[] PlatePosition => _platePosition;
    
    [SerializeField] private Transform[] _chairPosition;
    [SerializeField] private Transform[] _platePosition;
}
