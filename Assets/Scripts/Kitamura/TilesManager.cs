using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using RandomExtensions;
using Scriptable;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesManager : MonoBehaviour
{
    [SerializeField, InlineEditor] private TilePrefabsAsset tilePrefabs;
    [SerializeField] private AudioClip[] flipAudios;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField] private SoldierGameManager gameManager;

    [SerializeField] private ItemStackManager _itemStackManager;
    [SerializeField] private SoldierUIManager Screen;
    [SerializeField] private SoldierUIManager Log;
    [SerializeField]
    private InputActionReference cameraInput;//マウスの入力を取得
    [SerializeField]
    private InputActionReference moveInput;//キャラクターの移動を取得
    [SerializeField] private InputActionReference cameraLock;

    // ゲームクリア時のサブジェクト
    private readonly Subject<Unit> _gameClear = new();
    public Observable<Unit> GameClear => _gameClear;

    // 取得だけpublicとして公開。セットはprivate
    public TileController[] TileControllers { get; private set; }
    public MapRateAsset MapRate { get; private set; }
    public MapInfo MapInfo { get; private set; }
    private ItemInfo _itemInfo;
    [SerializeField] private TimerManager TimerManager;

    private int _noBombCount = 0;
    private int _comboCount = 0;
    private const float ComboTime = 2.5f;
    
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
                    Debug.Log(itemInfo.itemIcon);
                    blueInstance.SetItemIcon(itemInfo.itemIcon.itemIconBlack);
                    // めくられたらアイテムを追加
                    blueInstance.OnFlipped.Subscribe(_ =>
                    {
                        _itemStackManager.AddItem(itemInfo.itemType, itemInfo.itemIcon.itemIconBlack);
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
        // タイルが爆弾出ないとき
        if (tileTile.TileType != TileType.Bomb)
        {
            Log.AddLog("That tile is safe!", ColorType.Blue);
            ComboTimer();
            _comboCount++;
            var soundCount = _comboCount < flipAudios.Length ? _comboCount : flipAudios.Length - 1;
            var _flipSound = flipAudios[soundCount];
            Debug.Log(_comboCount+":"+ soundCount);
            TileControllers[tileId].PlaySound(_flipSound);
            

            // 周囲のタイルの状況を調べる
            var bombSum = GetTileAroundSumByType(tileId, TileType.Bomb);
            
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
            }
        } else
        {
            Log.AddLog("It's Bomb!!", ColorType.Red);
        }
        //CheckForOnlyBombs();
    }

    private CancellationTokenSource _cts;
    private async void ComboTimer()
    {
        _cts?.Cancel();

        var newCancellation = new CancellationTokenSource();
        _cts = newCancellation;
        try
        {
            await UniTask.WaitForSeconds(ComboTime, cancellationToken: newCancellation.Token);
            _comboCount = 0;
        }
        catch
        {
        }
        finally
        {
            if(_cts == newCancellation)
            {
                _cts.Dispose();
                _cts = null;
            }
        }
    }
    
    // タイルタイルの周囲の爆弾の数を取得
    private int GetTileAroundSumByType(int tileId, TileType type)
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

    public List<TileController> GetTillAllByType(TileType type)
    {
        List<TileController> result = new ();

        foreach (var tileController in TileControllers)
        {
            result.Add(tileController);
        }

        return result;
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

    public void ViewBombNumbs()
    {
        foreach (var tileController in TileControllers)
        {
            tileController.FadeInNumbText();
        }
    }
}
