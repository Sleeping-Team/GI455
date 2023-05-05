using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class OrderQueue : MonoBehaviour
{
    [SerializeField] private float _wholeDuration;

    [SerializeField] private TMP_Text _number;
    
    [SerializeField] private Transform _top;
    [SerializeField] private Transform _middle;
    [SerializeField] private Transform _bottom;

    private float _topDistance;
    private float _middleDistance;
    private float _bottomDistance;
    
    [SerializeField] private float _topEndY;
    [SerializeField] private float _middleEndY;
    [SerializeField] private float _bottomEndY;

    private float _duration;

    private void Awake()
    {
        _duration = _wholeDuration / 3f;
    }

    public void Setup(int tableNumber)
    {
        _number.text = tableNumber.ToString();
    }

    [ContextMenu("Animate/Base")]
    private void BaseIn()
    {
        _top.transform.DOLocalMoveY(_topEndY, _duration);
    }
    
    [ContextMenu("Animate/Middle")]
    public void MiddleIn()
    {
        _middle.transform.DOLocalMoveY(_middleEndY, _duration);
    }
    
    [ContextMenu("Animate/Top")]
    private void TopIn()
    {
        _bottom.transform.DOLocalMoveY(_bottomEndY, _duration);
    }

    [ContextMenu("Animate/Sequence")]
    public void Sequence()
    {
        _top.DOLocalMoveY(_topEndY, _duration).OnComplete(
            ()=> _middle.DOLocalMoveY(_middleEndY, _duration).OnComplete(
                () => _bottom.DOLocalMoveY(_bottomEndY, _duration)
                )
            );
    }
}
