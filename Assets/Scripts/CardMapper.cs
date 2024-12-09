using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SquareMap
{
    public readonly CardRateAsset CardRateAsset;
    public readonly int Width;
    public readonly int Height;
    private readonly int[] _map;
    
    public int[] Map => _map;

    public SquareMap(CardRateAsset cardRateAsset, Vector2Int start, int width, int height)
    {
        CardRateAsset = cardRateAsset;
        Width = width;
        Height = height;
        
        _map = new int[Width * Height];
        GenerateMap(start);
    }
    

    private void GenerateMap(Vector2Int start)
    {
        var startId = MapCalculation.GetCardId(start, Width);
        var aroundCardIds = MapCalculation.GetAroundCardIds(startId, Width, _map.Length);
        for (var i = 0; i < _map.Length; i++)
        {
            if (i == startId || aroundCardIds.Contains(i))
            {
                _map[i] = CardRateAsset._defaultCard;
            }
            else
            {
                _map[i] = CardRateAsset.GetRandomIndex();
            }
        }
    }
}

public static class MapCalculation
{
    public static Vector2Int GetCardPosition(int cardId, int width)
    {
        var x = cardId / width;
        var y = cardId % width;
        return new Vector2Int(x, y);
    }
    
    public static int GetCardId(Vector2Int position, int width)
    {
        return position.x * width + position.y;
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
            // タイルカードのIDを取得
            var aroundCardId = GetCardId(aroundPosition, width);
            if(aroundCardId <= 0 || aroundCardId > length) continue;
            // 周囲のタイルカードのIDを追加
            aroundCardIds.Add(aroundCardId);
        }
        return aroundCardIds;
    }

}
