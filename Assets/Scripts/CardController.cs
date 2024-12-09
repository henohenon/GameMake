using System.Collections;
using System.Collections.Generic;
using R3;
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
    
    private Subject<int> _onFlipped = new Subject<int>();
    public Observable<int> OnFlipped => _onFlipped;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // カードの色を裏面の色にする
        _meshRenderer.material.color = _backColor;
    }

    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _cardId;
    public void Initialize(int cardId) // monobehaviourはコンストラクタを持てない(どうして)ので初期化メソッドを作る
    {
        _cardId = cardId;
    }
    
    public void FlipCard()
    {
        if(_isFlipped) return; // もし裏返していたら何もしない
        _isFlipped = true;
        
        // カードを回転させる
        transform.Rotate(180, 0, 0);
        // カードの色を変える
        _meshRenderer.material.color = _frontColor;
        
        // イベントを発行
        _onFlipped.OnNext(_cardId);
    }
}
