using System;
using System.Collections.Generic;
using RandomExtensions;
using RandomExtensions.Collections;
using UnityEngine;
using Scriptable;

public static class MapTileCalc
{
    public static Vector2Int GetTilePosition(int tileId, MapLength length)
    {
        var x = tileId % length.Width;// 余りを求める
        var y = tileId / length.Width;// 商を求める
        return new Vector2Int(x, y);
    }
    
    public static int GetTileId(Vector2Int position, MapLength length)
    {
        // タイルタイルの座標が範囲外の場合はスキップ
        if(position.x < 0 || position.x >= length.Width) return -1;
        if (position.y < 0 || position.y >= length.Height) return -1;
        // タイルタイルのIDを取得
        var calcId = position.y * length.Width + position.x;
        // タイルタイルのIDが範囲外の場合はスキップ
        if(calcId < 0 || calcId >= length.TotalLength) return -1;
        
        return calcId;
    }
    
    public static Vector2Int GetInvertXPosition(Vector2Int position, MapLength length)
    {
        return new Vector2Int(length.Width - position.x - 1, position.y);
    }
    
    public static int GetInvertXId(Vector2Int position, MapLength length)
    {
        var invertPosition = GetInvertXPosition(position, length);
        return GetTileId(invertPosition, length);
    }
    
    public static Vector2Int GetInvertXPosition(int tileId, MapLength length)
    {
        var tilePosition = GetTilePosition(tileId, length);
        return GetInvertXPosition(tilePosition, length);
    }

    public static int GetInvertXId(int tileId, MapLength length)
    {
        var tilePosition = GetTilePosition(tileId, length);
        return GetInvertXId(tilePosition, length);
    }


    public static Vector2Int GetInvertYPosition(Vector2Int position, MapLength length)
    {
        return new Vector2Int(position.x, length.Height - position.y - 1);
    }
    
    public static int GetInvertYId(Vector2Int position, MapLength length)
    {
        var invertPosition = GetInvertYPosition(position, length);
        return GetTileId(invertPosition, length);
    }
    
    public static Vector2Int GetInvertYPosition(int tileId, MapLength length)
    {
        return GetInvertYPosition(GetTilePosition(tileId, length), length);
    }

    public static int GetInvertYId(int tileId, MapLength length)
    {
        var tilePosition = GetTilePosition(tileId, length);
        return GetInvertYId(tilePosition, length);
    }
    
    public static List<int> GetAroundTileIds(int tileId, MapLength length)
    {
        // 指定のタイルタイルの座標を取得
        var position = GetTilePosition(tileId, length);
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
            var aroundTileId = GetTileId(aroundPosition, length);
            // タイルタイルのIDが範囲外の場合はスキップ
            if (aroundTileId == -1) continue;
            aroundTileIds.Add(aroundTileId);
        }
        return aroundTileIds;
    }
}
