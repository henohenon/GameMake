using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using Scriptable;
using UnityEditor;

public class MapInfo
{
    public readonly MapLength MapLength;
    public readonly int[] Tiles;
    
    public MapInfo(EachTileInfo[] tileRateInfos, IRandom random, int width, int height)
    {
        MapLength = new MapLength(width, height);
        Tiles = new int[MapLength.TotalLength];
        
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }
    
    public MapInfo(EachTileInfo[] tileRateInfos, IRandom random, int length)
    {
        MapLength = new MapLength(length);
        Tiles = new int[MapLength.TotalLength];
        
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }
    
    public MapInfo(EachTileInfo[] tileRateInfos, int width, int height)
    {
        var random = RandomEx.Shared;
        MapLength = new MapLength(width, height);
        Tiles = new int[MapLength.TotalLength];
        
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }
    
    public MapInfo(EachTileInfo[] tileRateInfos, int length)
    {
        var random = RandomEx.Shared;
        MapLength = new MapLength(length);
        Tiles = new int[MapLength.TotalLength];
        
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }


    private void CreateRandomMap(IRandom random, EachTileInfo[] tileRateInfos)
    {
        // レートアセットからタイルの種類の重みつきリストを生成
        var weightList = new WeightedList<int>();
        for(var i = 0; i < tileRateInfos.Length; i++)
        {
            var tileInfo = tileRateInfos[i];
            weightList.Add(i, tileInfo.rate);
        }
        // 重みつきリストからランダムにタイルを選択
        for (var i = 0; i < Tiles.Length; i++)
        {
            Tiles[i] = weightList.GetItem(random);
        }
    }
}

public class MapLength
{
    public readonly int Width;
    public readonly int Height;
    public int TotalLength => Width * Height;

    public MapLength(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public MapLength(int length)
    {
        Width = length;
        Height = length;
    }
}
