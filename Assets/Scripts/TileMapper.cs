using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SquareTileMap
{
    public readonly GameRateAsset Asset;
    public readonly int Width;
    public readonly int Height;
    private readonly int[] _map;
    
    public int[] Map => _map;

    // コンストラクタ
    public SquareTileMap(GameRateAsset asset, Vector2Int start, int width, int height)
    {
        Asset = asset;
        Width = width;
        Height = height;
        
        _map = new int[Width * Height];
        GenerateMap(start);
    }
    

    private void GenerateMap(Vector2Int start)
    {
        // スタート地点のタイルIDを取得
        var startId = MapTileCalc.GetTileId(start, Width);
        
        // マップを生成
        for (var i = 0; i < _map.Length; i++)
        {
            // スタート地点のタイルIDの場合
            if (i == startId)
            {
                // デフォルトのタイルを設定
                _map[i] = Asset._defaultTile;
            }
            else
            {
                // ランダムなタイルを設定
                _map[i] = Asset.GetRandomIndex();
            }
        }
    }
}

public static class MapTileCalc
{
    public static Vector2Int GetTilePosition(int tileId, int width)
    {
        var x = tileId % width;// 余りを求める
        var y = tileId / width;// 商を求める
        return new Vector2Int(x, y);
    }
    
    public static int GetTileId(Vector2Int position, int width)
    {
        return position.y * width + position.x;
    }
    
    public static Vector2Int GetInvertXPosition(Vector2Int position, int width)
    {
        return new Vector2Int(width - position.x - 1, position.y);
    }
    
    public static int GetInvertXId(Vector2Int position, int height)
    {
        var invertPosition = GetInvertXPosition(position, height);
        return GetTileId(invertPosition, height);
    }
    
    public static Vector2Int GetInvertXPosition(int tileId, int width)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertXPosition(tilePosition, width);
    }

    public static int GetInvertXId(int tileId, int width)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertXId(tilePosition, width);
    }
    
    
    public static Vector2Int GetInvertYPosition(Vector2Int position, int height)
    {
        return new Vector2Int(position.x, height - position.y - 1);
    }
    
    public static int GetInvertYId(Vector2Int position, int width)
    {
        var invertPosition = GetInvertYPosition(position, width);
        return GetTileId(invertPosition, width);
    }
    
    public static Vector2Int GetInvertYPosition(int tileId, int width, int height)
    {
        return GetInvertYPosition(GetTilePosition(tileId, width), height);
    }

    public static int GetInvertYId(int tileId, int width)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertYId(tilePosition, width);
    }
    
    public static List<int> GetAroundTileIds(int tileId, int width, int length)
    {
        // 指定のタイルタイルの座標を取得
        var position = GetTilePosition(tileId, width);
        // 指定のタイルタイルの周囲の座標の一覧を作成
        var aroundPositions = new []
        {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.right,
            position + Vector2Int.left,
            position + Vector2Int.up + Vector2Int.right,
            position + Vector2Int.up + Vector2Int.left,
            position + Vector2Int.down + Vector2Int.right,
            position + Vector2Int.down + Vector2Int.left
        };
        // 周囲のタイルタイルのIDを取得
        var aroundTileIds = new List<int>();
        foreach (var aroundPosition in aroundPositions)
        {
            // タイルタイルの座標が範囲外の場合はスキップ
            if(aroundPosition.x < 0 || aroundPosition.x >= width) continue;
            if(aroundPosition.y < 0 || aroundPosition.y >= length / width) continue;
            // タイルタイルのIDを取得
            var aroundTileId = GetTileId(aroundPosition, width);
            // タイルタイルのIDが範囲外の場合はスキップ
            if(aroundTileId < 0 || aroundTileId >= length) continue;
            // 周囲のタイルタイルのIDを追加
            aroundTileIds.Add(aroundTileId);
        }
        return aroundTileIds;
    }

}
