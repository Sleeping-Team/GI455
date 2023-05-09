using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerProperties : MonoBehaviour
{
    public float Speed => _speed;
    public float LookSmooth => _lookSmooth;
    
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _lookSmooth = 0.05f;
}
