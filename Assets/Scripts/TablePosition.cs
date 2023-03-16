using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePosition : MonoBehaviour
{
    public Transform[] ChairPosition => _chairPosition;
    
    [SerializeField] private Transform[] _chairPosition;
}
