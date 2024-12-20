using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SquareTileMap
{
    public readonly CardRateAsset Asset;
    public readonly int Width;
    public readonly int Height;
    private readonly int[] _map;
    
    public int[] Map => _map;

    // コンストラクタ
    public SquareTileMap(CardRateAsset asset, Vector2Int start, int width, int height)
    {
        Asset = asset;
        Width = width;
        Height = height;
        
        _map = new int[Width * Height];
        GenerateMap(start);
    }
    

    private void GenerateMap(Vector2Int start)
    {
        // スタート地点のカードIDを取得
        var startId = MapTileCalc.GetCardId(start, Width);
        
        // マップを生成
        for (var i = 0; i < _map.Length; i++)
        {
            // スタート地点のカードIDの場合
            if (i == startId)
            {
                // デフォルトのカードを設定
                _map[i] = Asset._defaultCard;
            }
            else
            {
                // ランダムなカードを設定
                _map[i] = Asset.GetRandomIndex();
            }
        }
    }
}

public static class MapTileCalc
{
    public static Vector2Int GetCardPosition(int cardId, int width)
    {
        var x = cardId % width;// 余りを求める
        var y = cardId / width;// 商を求める
        return new Vector2Int(x, y);
    }
    
    public static int GetCardId(Vector2Int position, int width)
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
        return GetCardId(invertPosition, height);
    }
    
    public static Vector2Int GetInvertXPosition(int cardId, int width)
    {
        var cardPosition = GetCardPosition(cardId, width);
        return GetInvertXPosition(cardPosition, width);
    }

    public static int GetInvertXId(int cardId, int width)
    {
        var cardPosition = GetCardPosition(cardId, width);
        return GetInvertXId(cardPosition, width);
    }
    
    
    public static Vector2Int GetInvertYPosition(Vector2Int position, int height)
    {
        return new Vector2Int(position.x, height - position.y - 1);
    }
    
    public static int GetInvertYId(Vector2Int position, int width)
    {
        var invertPosition = GetInvertYPosition(position, width);
        return GetCardId(invertPosition, width);
    }
    
    public static Vector2Int GetInvertYPosition(int cardId, int width, int height)
    {
        return GetInvertYPosition(GetCardPosition(cardId, width), height);
    }

    public static int GetInvertYId(int cardId, int width)
    {
        var cardPosition = GetCardPosition(cardId, width);
        return GetInvertYId(cardPosition, width);
    }
    
    public static List<int> GetAroundCardIds(int cardId, int width, int length)
    {
        // 指定のタイルカードの座標を取得
        var position = GetCardPosition(cardId, width);
        // 指定のタイルカードの周囲の座標の一覧を作成
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
        // 周囲のタイルカードのIDを取得
        var aroundCardIds = new List<int>();
        foreach (var aroundPosition in aroundPositions)
        {
            // タイルカードの座標が範囲外の場合はスキップ
            if(aroundPosition.x < 0 || aroundPosition.x >= width) continue;
            if(aroundPosition.y < 0 || aroundPosition.y >= length / width) continue;
            // タイルカードのIDを取得
            var aroundCardId = GetCardId(aroundPosition, width);
            // タイルカードのIDが範囲外の場合はスキップ
            if(aroundCardId < 0 || aroundCardId >= length) continue;
            // 周囲のタイルカードのIDを追加
            aroundCardIds.Add(aroundCardId);
        }
        return aroundCardIds;
    }

}
