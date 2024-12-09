using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))] // メッシュレンダラーを必須にする
public class CardController : MonoBehaviour
{
    [SerializeField]
    private Color _frontColor;
    [SerializeField]
    private Color _backColor;
    
    private MeshRenderer _meshRenderer;
    private bool _isFlipped = false;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // カードの色を裏面の色にする
        _meshRenderer.material.color = _backColor;
    }
    
    public void FlipCard()
    {
        if(_isFlipped) return; // もし裏返していたら何もしない
        _isFlipped = true;
        
        // カードを回転させる
        transform.Rotate(180, 0, 0);
        // カードの色を変える
        _meshRenderer.material.color = _frontColor;
    }
}
