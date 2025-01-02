using System;
using System.Collections.Generic;
using RandomExtensions;
using RandomExtensions.Collections;
using UnityEngine;
using Scriptable;

public static class MapTileCalc
{
    public static Vector2Int GetTilePosition(int tileId, int width)
    {
        var x = tileId % width;// 余りを求める
        var y = tileId / width;// 商を求める
        return new Vector2Int(x, y);
    }
    
    public static int GetTileId(Vector2Int position, int width, int height)
    {
        // タイルタイルの座標が範囲外の場合はスキップ
        if(position.x < 0 || position.x >= width) return -1;
        if (position.y < 0 || position.y >= height) return -1;
        // タイルタイルのIDを取得
        var calcId = position.y * width + position.x;
        // タイルタイルのIDが範囲外の場合はスキップ
        if(calcId < 0 || calcId >= width*height) return -1;
        
        return calcId;
    }
    
    public static Vector2Int GetInvertXPosition(Vector2Int position, int width)
    {
        return new Vector2Int(width - position.x - 1, position.y);
    }
    
    public static int GetInvertXId(Vector2Int position, int width, int height)
    {
        var invertPosition = GetInvertXPosition(position, height);
        return GetTileId(invertPosition, width, height);
    }
    
    public static Vector2Int GetInvertXPosition(int tileId, int width)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertXPosition(tilePosition, width);
    }

    public static int GetInvertXId(int tileId, int width, int height)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertXId(tilePosition, width, height);
    }
    
    
    public static Vector2Int GetInvertYPosition(Vector2Int position, int height)
    {
        return new Vector2Int(position.x, height - position.y - 1);
    }
    
    public static int GetInvertYId(Vector2Int position, int width, int height)
    {
        var invertPosition = GetInvertYPosition(position, width);
        return GetTileId(invertPosition, width, height);
    }
    
    public static Vector2Int GetInvertYPosition(int tileId, int width, int height)
    {
        return GetInvertYPosition(GetTilePosition(tileId, width), height);
    }

    public static int GetInvertYId(int tileId, int width, int height)
    {
        var tilePosition = GetTilePosition(tileId, width);
        return GetInvertYId(tilePosition, width, height);
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
            // タイルタイルのIDを取得
            var aroundTileId = GetTileId(aroundPosition, width, length / width);
            // タイルタイルのIDが範囲外の場合はスキップ
            if (aroundTileId == -1) continue;
            aroundTileIds.Add(aroundTileId);
        }
        return aroundTileIds;
    }
}
