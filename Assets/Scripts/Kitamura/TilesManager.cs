using Alchemy.Inspector;
using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    [SerializeField, InlineEditor] private TilePrefabsAsset tilePrefabs;
    [SerializeField]
    private PlayerController playerController;

    [SerializeField] private ItemStackManager _itemStackManager;
    [SerializeField] private SoldierUIManager Screen;

    // ゲームクリア時のサブジェクト
    private readonly Subject<Unit> _gameClear = new();
    public Observable<Unit> GameClear => _gameClear;

    // 取得だけpublicとして公開。セットはprivate
    public TileController[] TileControllers { get; private set; }
    public MapRateAsset MapRate { get; private set; }
    public MapInfo MapInfo { get; private set; }
    private ItemInfo _itemInfo;
    
    private int _noBombCount = 0;
    
    public void Generate3dMap(MapRateAsset rate, GameInfo info)
    {
        MapRate = rate;
        MapInfo = info.MapInfo;
        _itemInfo = info.ItemInfo;

        if (TileControllers != null)
        {
            // 既存のタイルを破棄
            foreach (var tile in TileControllers)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }

        // 配列の初期化
        TileControllers = new TileController[MapInfo.MapLength.TotalLength];
        
        // 爆弾以外のタイルの数をリセット
        _noBombCount = 0;
        
        // マップのデータに沿ってタイルタイルプレファブのインスタンスを生成
        for (int i = 0; i < MapInfo.Tiles.Length; i ++)
        {
            // タイル情報を取得
            var tileInfo = rate.tileRateInfos[MapInfo.Tiles[i]];
            
            // タイルを生成。種類によって使用するプレファブを変える
            TileController instance = null;
            // タイルごとに特殊な処理がある場合はここで実装する
            switch (tileInfo.tileType)
            {
                case TileType.Safety:
                {
                    var prefab = tilePrefabs.safeTilePrefab;
                    instance = Instantiate(prefab);
                    break;
                }
                case TileType.Bomb:
                {
                    var prefab = tilePrefabs.bombTilePrefab;
                    instance = Instantiate(prefab);
                    break;
                }
                case TileType.BlueItem:
                {
                    // アイコンを設定する
                    var prefab = tilePrefabs.blueItemTilePrefab;
                    var blueInstance = Instantiate(prefab);
                    var itemInfo = _itemInfo.GetRandomBlueItem();
                    blueInstance.SetItemIcon(itemInfo.itemIcon);
                    // めくられたらアイテムを追加
                    blueInstance.OnFlipped.Subscribe(_ =>
                    {
                        _itemStackManager.AddItem(itemInfo.itemType);
                    });
                    instance = blueInstance;
                    break;
                }
            }
            
            // タイルの座標を計算
            var tilePosition = MapTileCalc.GetTilePosition(i, MapInfo.MapLength); // タイルタイルの座標を取得
            var tileX = tilePosition.x - MapInfo.MapLength.Width / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
            var tileZ = tilePosition.y - MapInfo.MapLength.Height / 2;
            var tileVector = new Vector3(tileX, 0, tileZ);
            var tileQuaternion = Quaternion.identity; // 回転なし、0度の状態
            // タイルの座標を設定
            instance.transform.position = tileVector;
            instance.transform.rotation = tileQuaternion;
            
            // このスクリプトがアタッチされているオブジェクトの子にする
            instance.transform.SetParent(transform);
            
            // ランダムにタイルの見た目オブジェクトを生成
            var tileObjIdx = RandomEx.Shared.NextInt(0, MapRate.randomTiles.Length);
            var tileObjInstance = Instantiate(MapRate.randomTiles[tileObjIdx]);
            
            // TileControllerを初期化
            instance.Initialize(i, tileInfo.tileType, tileObjInstance);
            
            // TileControllerを配列に格納
            TileControllers[i] = instance;
        
            // タイルが裏返されたときのイベントを購読し、タイルが裏返されたときの処理を実行
            instance.OnFlipped.Subscribe(OnTileFlipped);
            
            // 爆弾以外のタイルの数をカウント
            if (tileInfo.tileType != TileType.Bomb)
            {
                _noBombCount++;
            }
        }
    }
    
    // タイルタイルが裏返されたときの処理
    private void OnTileFlipped(int tileId)
    {
        // タイルのインスタンスをキャッシュ的な感じで取得
        var tileTile = TileControllers[tileId];
        // タイルタイルが爆弾の場合はゲームオーバー
        if (tileTile.TileType == TileType.Bomb)
        {
            Debug.Log("Game Over");
            // プレイヤーの位置とタイルタイルの位置から方向を計算し、プレイヤーに衝撃を与える
            var playerPos = playerController.transform.position;
            var tilePos = tileTile.transform.position;
            var direction = (playerPos - tilePos).normalized;
            playerController.Impact(direction);
            Screen.SetHiddenCredit(InPlayScreenType.GameOver,false);
        }
        else
        {
            // 周囲のタイルの状況を調べる
            var bombSum = GetTileAroundTypeSum(tileId, TileType.Bomb);
            
            // 周囲がすべて安全な場合
            if (bombSum == 0)
            {
                // 周囲のタイルタイルを裏返す
                var aroundTiles = MapTileCalc.GetAroundTileIds(tileId, MapInfo.MapLength); // 周辺取得何回か繰り返しちゃってるがまぁ気にしないこととする
                foreach (var aroundTileId in aroundTiles)
                {
                    // 安全なタイルなら開ける
                    var tileInfoIndex = MapInfo.Tiles[aroundTileId];
                    var tileType = MapRate.tileRateInfos[tileInfoIndex].tileType;
                    if (tileType == TileType.Safety)
                    {
                        TileControllers[aroundTileId].Open();
                    }
                }
            }
            else
            {
                // 周囲の爆弾の数を表示
                tileTile.SetText(bombSum.ToString());
            }
            
            // 爆弾以外のタイルの数を減らす
            _noBombCount--;
            // 爆弾以外のタイルがなくなった場合はクリア
            if (_noBombCount == 0)
            {
                Debug.Log("Game Clear");
                _gameClear.OnNext(Unit.Default);
                Screen.SetHiddenCredit(InPlayScreenType.GameClear, false);
            }
        }
        //CheckForOnlyBombs();
    }
    
    // タイルタイルの周囲の爆弾の数を取得
    private int GetTileAroundTypeSum(int tileId, TileType type)
    {
        var sum = 0;

        // 周囲のタイルタイルのIDを取得
        var aroundTileIds = MapTileCalc.GetAroundTileIds(tileId, MapInfo.MapLength);
        Debug.Log(aroundTileIds);
        
        
        // 周囲のタイルタイルを調べる
        foreach (var aroundTileId in aroundTileIds)
        {
            // 指定のタイプであれば加算
            if (TileControllers[aroundTileId].TileType == type)
            {
                sum++;
            }
        }

        return sum;
    }

    public TileType GetTileIdType(int tileId)
    {
        var infoIndex = MapInfo.Tiles[tileId];
        var tileType = MapRate.tileRateInfos[infoIndex].tileType;
        return tileType;
    }
    
    public Vector2Int GetMapPosition(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x + (float)MapInfo.MapLength.Width / 2);
        var z = Mathf.FloorToInt(position.z + (float)MapInfo.MapLength.Height / 2);
            
        return new Vector2Int(x, z);
    }
}
