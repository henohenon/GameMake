using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

public class TilesManager : MonoBehaviour
{
    [SerializeField]
    private GameRateAsset firstAsset;
    [SerializeField]
    private GameRateAsset gameRate;
    [SerializeField]
    private int length = 9; // 一片の長さ。length*lengthのマスが生成される
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private UIManager uiManager;

    private TileController[] _tiles;
    private SquareTileMap _squareTileMap;

    // Start is called before the first frame update
    void Start()
    {
        // タイルタイルの配列指定の長さで初期化
        _tiles = new TileController[length * length];
        // 開始位置判定用のタイルタイルを生成
        CreateMaps(firstAsset, Vector2Int.zero);
        // マップをUIに書き込む
        uiManager.WriteMap(_squareTileMap);
    }

    private void CreateMaps(GameRateAsset asset, Vector2Int startPos)
    {
        // タイルタイルの配列を生成
        foreach (var tile in _tiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        
        // アセットと開始位置、長さを指定してMapを生成
        _squareTileMap = new SquareTileMap(asset, startPos, length, length);
        
        // マップのデータに沿ってタイルタイルプレファブのインスタンスを生成
        for (int i = 0; i < _squareTileMap.Map.Length; i ++)
        {
            // タイル情報を取得
            var tileInfo = asset.tileInfos[_squareTileMap.Map[i]];
            // タイルタイルの座標を計算
            var tilePosition = MapTileCalc.GetTilePosition(i, length); // タイルタイルの座標を取得
            var tileX = tilePosition.x - _squareTileMap.Width / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
            var tileZ = tilePosition.y - _squareTileMap.Height / 2;
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
            
            // 最初の位置選択の場合
            if (tileInfo.tileType == TileType.First)
            {
                // タイルタイルが裏返されたときのイベントを購読し、初期位置選択時の処理を実行
                instantiate.OnFlipped.Subscribe(OnFirstFlipped);
            }
            else
            {
                // タイルタイルが裏返されたときのイベントを購読し、(本番)タイルタイルが裏返されたときの処理を実行
                instantiate.OnFlipped.Subscribe(OnTileFlipped);
            }
        }
    }

    // タイルタイルが裏返されたときの処理
    private async void OnFirstFlipped(int tileId)
    {
        // 選択されたタイルから初期位置を取得
        var tilePosition = MapTileCalc.GetTilePosition(tileId, length);
        // 本番タイルタイルを生成
        CreateMaps(gameRate, tilePosition);
        
        // タイルマップのUIをクリア
        uiManager.ClearMaps();
        // タイルマップの配列を作成
        var maps = new SquareTileMap[]
        {
            _squareTileMap,
            new (gameRate, tilePosition, length, length),
            new (gameRate, tilePosition, length, length)
        };
        // タイルマップをシャッフル
        var shuffledMaps = maps.OrderBy(m => Random.value).ToArray();
        
        // タイルマップをUIに書き込む
        foreach (var map in shuffledMaps)
        {
            uiManager.WriteMap(map);
        }

        // タイルインスタンスのStartメソッドが呼ばれるのを待つため、1フレーム待機(Startをなくしたほうがきれいではある)
        await UniTask.DelayFrame(1);
        // 初期位置(選択された)のタイルを裏返す
        _tiles[tileId].Flip();
    }
    
    // タイルタイルが裏返されたときの処理
    private void OnTileFlipped(int tileId)
    {
        // タイルのインスタンスをキャッシュ的な感じで取得
        var tileTile = _tiles[tileId];
        // タイルタイルが爆弾の場合はゲームオーバー
        if (tileTile.TileType == TileType.Bomb)
        {
            //Debug.Log("Game Over");
            // プレイヤーの位置とタイルタイルの位置から方向を計算し、プレイヤーに衝撃を与える
            var playerPos = playerController.transform.position;
            var tilePos = tileTile.transform.position;
            var direction = (playerPos - tilePos).normalized;
            playerController.Impact(direction);
            Debug.Log("if");
            
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
                Debug.Log("else if");
            }
            else
            {
                // 周囲に爆弾がある場合は周囲の爆弾の数を表示
                tileTile.SetText(sum.ToString());
                Debug.Log("else else");
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
    /*
    private void CheckForOnlyBombs()
{
    if (onlyBombs)
    {
        Debug.Log("Clear! All tiles are bombs.");
        // クリア処理をここに追加
    }
}
    */
}
