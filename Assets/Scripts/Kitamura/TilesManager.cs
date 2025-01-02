using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using R3;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilesManager : MonoBehaviour
{
    [SerializeField, InlineEditor]
    private GameRateAsset gameRate;
    [SerializeField, InlineEditor] private TilePrefabsAsset tilePrefabs;
    [SerializeField]
    private int length = 9; // 一片の長さ。length*lengthのマスが生成される
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField] private InputActionReference openTile;

    private TileController[] _tiles;
    private GameInfo _gameInfo;

    // Start is called before the first frame update
    void Start()
    {
        // タイルタイルの配列指定の長さで初期化
        _tiles = new TileController[length * length];
        
        // 乱数のシードを生成
        var seed = GenerateSeed();
        Debug.Log("Seed: " + seed);
        // ゲーム情報を初期化
        _gameInfo = new GameInfo(length, length, gameRate, seed);
        // ログを出力
        InfoLogger.LogItem(_gameInfo.ItemInfo, gameRate.itemRateAsset);
        InfoLogger.LogMap(_gameInfo.MapInfo, gameRate);
        
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
        
        openTile.action.started += OpenTileCallback;
        openTile.action.Enable();
    }
    
    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(selectingTile)
            {
                Debug.Log("Open");
                selectingTile.Open();
            }
        }
    }
    
    private TileController selectingTile;
    public void Update()
    {
        var playerPosition = playerController.transform.position;
        var playerDirection = playerController.transform.forward;
        playerDirection.y = 0;
        playerDirection.Normalize();
        var playerForwardPosition = playerPosition + playerDirection;
        
        var highLightPosition = GetPosition(playerForwardPosition);
        
        var positionTileId = MapTileCalc.GetTileId(highLightPosition, _gameInfo.MapInfo.Width, _gameInfo.MapInfo.Height);

        if (positionTileId == -1)
        {
            if (selectingTile)
            {
                selectingTile.Select(false);
                selectingTile = null;
            }
            return;
        }
        
        var tile = _tiles[positionTileId];

        if (tile != selectingTile)
        {
            selectingTile?.Select(false);
            selectingTile = tile;
            selectingTile.Select();
        }
    }

    private int noBombCount = 0;
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
        
        // 爆弾以外のタイルの数をリセット
        noBombCount = 0;
        
        // マップのデータに沿ってタイルタイルプレファブのインスタンスを生成
        for (int i = 0; i < map.Tiles.Length; i ++)
        {
            // タイル情報を取得
            var tileInfo = asset.tileRateInfos[map.Tiles[i]];
            
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
                    blueInstance.SetItemIcon("heno");
                    instance = blueInstance;
                    break;
                }
            }
            
            // タイルの座標を計算
            var tilePosition = MapTileCalc.GetTilePosition(i, length); // タイルタイルの座標を取得
            var tileX = tilePosition.x - map.Width / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
            var tileZ = tilePosition.y - map.Height / 2;
            var tileVector = new Vector3(tileX, 0, tileZ);
            var tileQuaternion = Quaternion.identity; // 回転なし、0度の状態
            // タイルの座標を設定
            instance.transform.position = tileVector;
            instance.transform.rotation = tileQuaternion;
            
            // このスクリプトがアタッチされているオブジェクトの子にする
            instance.transform.SetParent(transform);
            
            // ランダムにタイルの見た目オブジェクトを生成
            var tileObjIdx = RandomEx.Shared.NextInt(0, gameRate.randomTiles.Length);
            var tileObjInstance = Instantiate(gameRate.randomTiles[tileObjIdx]);
            
            // TileControllerを初期化
            instance.Initialize(i, tileInfo.tileType, tileObjInstance);
            
            // TileControllerを配列に格納
            _tiles[i] = instance;
        
            // タイルが裏返されたときのイベントを購読し、タイルが裏返されたときの処理を実行
            instance.OnFlipped.Subscribe(OnTileFlipped);
            
            // 爆弾以外のタイルの数をカウント
            if (tileInfo.tileType != TileType.Bomb)
            {
                noBombCount++;
            }
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
                    _tiles[aroundTileId].Open();
                }
            }
            else
            {
                // 周囲に爆弾がある場合は周囲の爆弾の数を表示
                tileTile.SetText(sum.ToString());
            }
            
            // 爆弾以外のタイルの数を減らす
            noBombCount--;
            // 爆弾以外のタイルがなくなった場合はクリア
            if (noBombCount == 0)
            {
                Debug.Log("Game Clear");
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
    public Vector2Int GetPosition(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x + (float)_gameInfo.MapInfo.Width / 2);
        var z = Mathf.FloorToInt(position.z + (float)_gameInfo.MapInfo.Height / 2);
            
        return new Vector2Int(x, z);
    }
}
