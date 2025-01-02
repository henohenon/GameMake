using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;
    
    private bool _isOpened = false;
    private GameObject _tileObject;
    
    // タイルが裏返されたときのイベント
    private Subject<int> _onFlipped = new ();
    // 購買のみを公開
    public Observable<int> OnFlipped => _onFlipped;
    public int defaultLayer;
    public int selectedLayer;
    
    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _tileId;
    public TileType TileType { get; private set; } // getはpublic、setはprivate
    
    
    private float[] _tileRotationY = {0, 90, 180, 270};
    // monobehaviourはコンストラクタを持てない(どうして)ので初期化メソッド
    public void Initialize(int tileId, TileType type, GameObject tileObject)
    {
        gameObject.name = $"Tile_{tileId}";
        _tileId = tileId;
        TileType = type;
        _tileObject = tileObject;
        
        _tileObject.transform.SetParent(transform);
        var randomRotation = _tileRotationY[RandomEx.Shared.NextInt(0, _tileRotationY.Length)];
        _tileObject.transform.localRotation = Quaternion.Euler(-90, randomRotation, 0);
        _tileObject.transform.localPosition = Vector3.zero;
    }
    
    public virtual void Open()
    {
        if(_isOpened) return; // もし裏返していたら何もしない
        _isOpened = true;
        
        // タイルを消す
        _tileObject.SetActive(false);
        // イベントを発行
        _onFlipped.OnNext(_tileId);
    }
    
    public void Select(bool isSelected = true)
    {
        if(_isOpened) return; // もし裏返していたら何もしない
        _tileObject.layer = isSelected ? selectedLayer : defaultLayer;
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
