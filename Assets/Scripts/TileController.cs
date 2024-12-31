using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    private Color _frontColor;
    [SerializeField]
    private Color _backColor;
    
    [SerializeField]
    private TextMesh _textMesh;
    
    private MeshRenderer _meshRenderer;
    private bool _isFlipped = false;
    
    // タイルが裏返されたときのイベント
    private Subject<int> _onFlipped = new ();
    // 購買のみを公開
    public Observable<int> OnFlipped => _onFlipped;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // タイルの色を裏面の色にする
        _meshRenderer.material.color = _backColor;
        _textMesh.text = "";
    }

    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _tileId;
    public TileType TileType { get; private set; } // getはpublic、setはprivate
    // monobehaviourはコンストラクタを持てない(どうして)ので初期化メソッド
    public void Initialize(int tileId, TileType type) 
    {
        gameObject.name = $"Tile_{tileId}";
        _tileId = tileId;
        TileType = type;
    }
    
    public void Flip()
    {
        if(_isFlipped) return; // もし裏返していたら何もしない
        _isFlipped = true;
        
        // タイルを回転させる
        transform.Rotate(180, 0, 0);
        // タイルの色を変える
        _meshRenderer.material.color = _frontColor;
        
        // イベントを発行
        _onFlipped.OnNext(_tileId);
    }
    
    public void SetText(string text)
    {
        _textMesh.text = text;
    }
}
