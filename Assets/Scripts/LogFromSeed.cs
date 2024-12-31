using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;

public class LogFromSeed : MonoBehaviour
{
    [SerializeField, AssetsOnly]
    private GameRateAsset gameRate;
    
    [Button]
    public void Log(uint seed)
    {
        var gameInfo = new GameInfo(9, gameRate, seed);
        InfoLogger.LogMap(gameInfo.MapInfo, gameRate);
        InfoLogger.LogItem(gameInfo.ItemInfo, gameRate.itemRateAsset);
    }
}
