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
    
    public Vector2Int GetCardPosition(int cardId)
    {
        var x = cardId / Width;
        var y = cardId % Width;
        return new Vector2Int(x, y);
    }
    
    public int GetCardId(Vector2Int position)
    {
        return position.x * Width + position.y;
    }

    public List<int> GetAroundCardIds(int cardId)
    {
        // 指定のタイルカードの座標を取得
        var position = GetCardPosition(cardId);
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
            // タイルカードが存在しない場合はスキップ
            if (aroundPosition.x < 0 || aroundPosition.x >= Width || aroundPosition.y < 0 || aroundPosition.y >= Width)
            {
                continue;
            }
            // タイルカードのIDを取得
            var aroundCardId = GetCardId(aroundPosition);
            // 周囲のタイルカードのIDを追加
            aroundCardIds.Add(aroundCardId);
        }
        return aroundCardIds;
    }
    
    private void GenerateMap(Vector2Int start)
    {
        var startId = GetCardId(start);
        var aroundCardIds = GetAroundCardIds(startId);
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
