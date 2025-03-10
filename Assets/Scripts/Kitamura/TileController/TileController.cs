using LitMotion;
using LitMotion.Extensions;
using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TileController : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;
    [SerializeField] private GameObject selectObj;
    [SerializeField] private GameObject openObj;

    protected AudioSource AudioSource;

    private TileState _tileState;
    private GameObject _tileObject;
    private FlagController _flagObject;
    
    // タイルが裏返されたときのイベント
    private Subject<int> _onFlipped = new ();
    // 購買のみを公開
    public Observable<int> OnFlipped => _onFlipped;
    
    // 余りこの辺は持たせたくはないが今回は分かりやすさを重視ということで
    private int _tileId;
    public TileType TileType { get; private set; } // getはpublic、setはprivate
    
    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        selectObj.SetActive(false);
    }
    
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
        // 選択を消す
        selectObj.SetActive(false);
        // 開いた時のオブジェクトを表示
        openObj.SetActive(true);
        // イベントを発行
        _onFlipped.OnNext(_tileId);

        return true;
    }

    public void ToggleFlag(FlagController prefab)
    {
        // もし裏返していたら何もしない
        if(_tileState == TileState.Open) return;

        // もし旗オブジェクトがあれば消す
        if (_flagObject)
        {
            _flagObject.Remove();
            _flagObject = null;
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
        selectObj.SetActive(isSelected);
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource.clip= clip;
        AudioSource.Play();
    }

    public void FadeInNumbText()
    {
        LMotion.Create(Color.clear, Color.white, 0.5f).Bind(textMesh, (x, target) =>
        {
            target.color = x;
        });
    }

    private enum TileState
    {
        Default,
        Open,
        Flag
    }
}

