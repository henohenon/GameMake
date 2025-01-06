using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using Scriptable;

public class GameInfo
{
    public readonly MapInfo MapInfo;
    public readonly ItemInfo ItemInfo;
    
    public GameInfo(GameRateAsset rateAsset, int width, int height, uint seed)
    {
        // シード値から乱数生成器を生成
        var random = new Xoshiro256StarStarRandom(seed);

        MapInfo = new MapInfo(rateAsset.mapRateAsset.tileRateInfos, random, width, height);
        ItemInfo = new ItemInfo(rateAsset.itemRateAsset, random);
    }
    
    
    public GameInfo(GameRateAsset rateAsset, int length, uint seed)
    {
        // シード値から乱数生成器を生成
        var random = new Xoshiro256StarStarRandom(seed);

        MapInfo = new MapInfo(rateAsset.mapRateAsset.tileRateInfos, random, length);
        ItemInfo = new ItemInfo(rateAsset.itemRateAsset, random);
    }
    
    public GameInfo(GameRateAsset rateAsset, int width, int height)
    {
        var random = RandomEx.Shared;

        MapInfo = new MapInfo(rateAsset.mapRateAsset.tileRateInfos, random, width, height);
        ItemInfo = new ItemInfo(rateAsset.itemRateAsset, random);
    }
    
    
    public GameInfo(GameRateAsset rateAsset, int length)
    {
        var random = RandomEx.Shared;

        MapInfo = new MapInfo(rateAsset.mapRateAsset.tileRateInfos, random, length);
        ItemInfo = new ItemInfo(rateAsset.itemRateAsset, random);
    }
}