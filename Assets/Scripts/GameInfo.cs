using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using Scriptable;

public class GameInfo
{
    public readonly MapInfo MapInfo;
    
    public GameInfo(int width, int height, GameRateAsset rateAsset, uint seed)
    {
        MapInfo = new MapInfo(width, height, rateAsset.tileRateInfos, seed);
    }
}
