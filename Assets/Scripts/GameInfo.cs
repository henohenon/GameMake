using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using Scriptable;

public class GameInfo
{
    public readonly MapInfo MapInfo;
    public readonly ItemInfo ItemInfo;
    
    public GameInfo(int width, int height, GameRateAsset rateAsset, uint seed)
    {
        // シード値から乱数生成器を生成
        var random = new Xoshiro256StarStarRandom(seed);

        MapInfo = new MapInfo(width, height, rateAsset.tileRateInfos, random);
        ItemInfo = new ItemInfo(rateAsset.itemRateAsset, random);
    }
}
