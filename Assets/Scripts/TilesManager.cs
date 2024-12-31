using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using UnityEngine;
using UnityEngine.Serialization;

public class TilesManager : MonoBehaviour
{
    [SerializeField]
    private GameRateAsset gameRate;
    [SerializeField]
    private int length = 9; // 一片の長さ。length*lengthのマスが生成される
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private UIManager uiManager;

    private TileController[] _tiles;
    private GameInfo _gameInfo;

    // Start is called before the first frame update
    void Start()
    {
        // タイルタイルの配列指定の長さで初期化
        _tiles = new TileController[length * length];
        
        // 乱数のシードを生成
        var seed = GenerateSeed();
        // ゲーム情報を初期化
        _gameInfo = new GameInfo(length, length, gameRate, seed);
        // 3dオブジェクトを生成
        Create3dMap(gameRate, _gameInfo.MapInfo);
        
        // タイルマップのUIをクリア
        uiManager.ClearMaps();
        // タイルマップの配列を作成
        var maps = new []
        {
            _gameInfo.MapInfo,
            new (length, gameRate.tileRateInfos),
            new (length, gameRate.tileRateInfos),
        }.AsEnumerable();

        // タイルマップをシャッフルしてUIに書き込む
        maps = maps.Shuffle().ToArray();
        foreach (var map in maps)
        {
            uiManager.WriteMap(gameRate, map);
        }
    }
    
    private void Create3dMap(GameRateAsset asset, MapInfo map)
    {
        // 既存のタイルを破棄
        foreach (var tile in _tiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        
        // マップのデータに沿ってタイルタイルプレファブのインスタンスを生成
        for (int i = 0; i < map.Tiles.Length; i ++)
        {
            // タイル情報を取得
            var tileInfo = asset.tileRateInfos[map.Tiles[i]];
            // タイルタイルの座標を計算
            var tilePosition = MapTileCalc.GetTilePosition(i, length); // タイルタイルの座標を取得
            var tileX = tilePosition.x - map.Width / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
            var tileZ = tilePosition.y - map.Height / 2;
            var tileVector = new Vector3(tileX, 0, tileZ);
            var tileQuaternion = Quaternion.identity; // 回転なし、0度の状態

            // タイルタイルを生成
            var instantiate = Instantiate(tileInfo.tilePrefab, tileVector, tileQuaternion);
            // タイルタイルを初期化
            instantiate.Initialize(i, tileInfo.tileType);
            // タイルタイルをこのスクリプトがアタッチされているオブジェクトの子にする
            instantiate.transform.SetParent(transform);
            // タイルタイルを配列に格納
            _tiles[i] = instantiate;
            
            // タイルタイルが裏返されたときのイベントを購読し、(本番)タイルタイルが裏返されたときの処理を実行
            instantiate.OnFlipped.Subscribe(OnTileFlipped);
        }
    }
    
    // タイルタイルが裏返されたときの処理
    private void OnTileFlipped(int tileId)
    {
        // タイルのインスタンスをキャッシュ的な感じで取得
        var tileTile = _tiles[tileId];
        // タイルタイルが爆弾の場合はゲームオーバー
        if (tileTile.TileType == TileType.Bomb)
        {
            Debug.Log("Game Over");
            // プレイヤーの位置とタイルタイルの位置から方向を計算し、プレイヤーに衝撃を与える
            var playerPos = playerController.transform.position;
            var tilePos = tileTile.transform.position;
            var direction = (playerPos - tilePos).normalized;
            playerController.Impact(direction);
            
        }
        else
        {
            // タイルタイルが爆弾でない場合は周囲の爆弾の数を取得
            var sum = GetTileAroundBombSum(tileId);

            // 周囲に爆弾がない場合
            if (sum == 0)
            {
                // 周囲のタイルタイルを裏返す
                var aroundTiles = MapTileCalc.GetAroundTileIds(tileId, length, length * length); // 周辺取得何回か繰り返しちゃってるがまぁ気にしないこととする
                foreach (var aroundTileId in aroundTiles)
                {
                    _tiles[aroundTileId].Flip();
                }
            }
            else
            {
                // 周囲に爆弾がある場合は周囲の爆弾の数を表示
                tileTile.SetText(sum.ToString());
            }
        }
        //CheckForOnlyBombs();
    }
    
    // タイルタイルの周囲の爆弾の数を取得
    private int GetTileAroundBombSum(int tileId)
    {
        var sum = 0;

        // 周囲のタイルタイルのIDを取得
        var aroundTileIds = MapTileCalc.GetAroundTileIds(tileId, length, length * length);
        Debug.Log(aroundTileIds);
        // 周囲のタイルタイルを調べる
        foreach (var aroundTileId in aroundTileIds)
        {
            // タイルタイルが爆弾の場合は加算
            if (_tiles[aroundTileId].TileType == TileType.Bomb)
            {
                sum++;
            }
        }

        return sum;
    }
    
    private uint GenerateSeed()
    {
        // 最大5桁の乱数を生成
        return RandomEx.Shared.NextUInt(10000);
    }
}
