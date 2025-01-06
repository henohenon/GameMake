using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    private TileState _tileState;
    private GameObject _tileObject;
    private GameObject _flagObject;
    
    // タイルが裏返されたときのイベント
    private Subject<int> _onFlipped = new ();
    // 購買のみを公開
    public Observable<int> OnFlipped => _onFlipped;
    public int defaultLayer;
    public int selectedLayer;
    
    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _tileId;
    public TileType TileType { get; private set; } // getはpublic、setはprivate
    
    
    private readonly float[] _tileRotationY = {0, 90, 180, 270};
    // monobehaviourはコンストラクタを持てない(どうして)ので初期化メソッド
    public void Initialize(int tileId, TileType type, GameObject tileObject)
    {
        gameObject.name = $"Tile_{tileId}";
        _tileId = tileId;
        TileType = type;
        _tileObject = tileObject;
        
        _tileObject.transform.SetParent(transform);
        var randomRotation = _tileRotationY[RandomEx.Shared.NextInt(0, _tileRotationY.Length)];
        _tileObject.transform.localRotation = Quaternion.Euler(0, randomRotation, 0);
        _tileObject.transform.localPosition = Vector3.zero;
        _tileObject.transform.localScale = Vector3.Scale(_tileObject.transform.localScale, new Vector3(1, 0.3f, 1));
    }
    
    public virtual bool Open()
    {
        // もし素の状態でなければ何もしない
        if(_tileState != TileState.Default) return false;
        _tileState = TileState.Open;
        
        // タイルを消す
        _tileObject.SetActive(false);
        // イベントを発行
        _onFlipped.OnNext(_tileId);

        return true;
    }

    public void ToggleFlag(GameObject prefab)
    {
        // もし裏返していたら何もしない
        if(_tileState == TileState.Open) return;

        // もし旗オブジェクトがあれば消す
        if (_flagObject)
        {
            Destroy(_flagObject);
        }
        if (_tileState == TileState.Flag)
        {
            _tileState = TileState.Default;
        }
        else if(_tileState == TileState.Default)
        {
            // 自身のことして旗オブジェクトを生成
            _flagObject = Instantiate(prefab, this.transform);
            _tileState = TileState.Flag;
        }
    }
    
    public void Select(bool isSelected = true)
    {
        // もし裏返していたら何もしない
        if(_tileState == TileState.Open) return;
        _tileObject.layer = isSelected ? selectedLayer : defaultLayer;
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
    }

    private enum TileState
    {
        Default,
        Open,
        Flag
    }
}

