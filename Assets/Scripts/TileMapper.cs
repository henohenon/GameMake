using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class SquareTileMap
{
    public int width;
    public int height;
    [SerializeField]
    private int[] map;
    
    public int[] Map => map;

    // スタート位置を指定するコンストラクタ
    public SquareTileMap(GameRateAsset asset, int width, int height, Vector2Int start)
    {
        this.width = width;
        this.height = height;
        
        map = new int[width * height];
        GenerateMap(asset, start);
    }
    // 完全ランダムなマップを生成するコンストラクタ
    public SquareTileMap(GameRateAsset asset, int width, int height)
    {
        this.width = width;
        this.height = height;
        
        map = new int[width * height];
        GenerateMap(asset);
    }
    
    // 完全ランダムなマップを生成
    private void GenerateMap(GameRateAsset asset)
    {
        for (var i = 0; i < map.Length; i++)
        {
            // ランダムなタイルを設定
            map[i] = asset.GetRandomIndex();
        }
    }
    
    // スタート位置が絶対安全なマップを生成
    private void GenerateMap(GameRateAsset asset, Vector2Int start)
    {
        // スタート地点のタイルIDを取得
        var startId = MapTileCalc.GetTileId(start, width);
        
        // マップを生成
        for (var i = 0; i < map.Length; i++)
        {
            // スタート地点のタイルIDの場合
            if (i == startId)
            {
                // デフォルトのタイルを設定
                map[i] = asset.defaultTile;
            }
            else
            {
                // ランダムなタイルを設定
                map[i] = asset.GetRandomIndex();
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
