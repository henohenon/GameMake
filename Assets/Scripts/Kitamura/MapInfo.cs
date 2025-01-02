using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using Scriptable;

public class MapInfo
{
    public readonly int Width;
    public readonly int Height;
    public readonly int[] Tiles;
    
    public MapInfo(int width, int height, EachTileInfo[] tileRateInfos, IRandom random)
    {
        Width = width;
        Height = height;
        Tiles = new int[width * height];
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }
    
    public MapInfo(int length, EachTileInfo[] tileRateInfos, IRandom random)
    {
        Width = length;
        Height = length;
        Tiles = new int[length * length];
        
        // マップの生成
        CreateRandomMap(random, tileRateInfos);
    }
    
    public MapInfo(int length, EachTileInfo[] tileRateInfos)
    {
        Width = length;
        Height = length;
        Tiles = new int[length * length];
        
        // マップの生成
        CreateRandomMap(RandomEx.Shared, tileRateInfos);
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
