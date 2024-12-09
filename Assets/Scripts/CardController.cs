using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField]
    private Color _frontColor;
    [SerializeField]
    private Color _backColor;
    
    [SerializeField]
    private TextMesh _textMesh;
    
    private MeshRenderer _meshRenderer;
    private bool _isFlipped = false;
    
    // カードが裏返されたときのイベント
    private Subject<int> _onFlipped = new ();
    // 購買のみを公開
    public Observable<int> OnFlipped => _onFlipped;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // カードの色を裏面の色にする
        _meshRenderer.material.color = _backColor;
        _textMesh.text = "";
    }

    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _cardId;
    public CardType CardType { get; private set; } // getはpublic、setはprivate
    // monobehaviourはコンストラクタを持てない(どうして)ので初期化メソッド
    public void Initialize(int cardId, CardType type) 
    {
        gameObject.name = $"Card_{cardId}";
        _cardId = cardId;
        CardType = type;
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
    
    public void SetText(string text)
    {
        _textMesh.text = text;
    }
}
