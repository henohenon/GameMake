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
    public void Log(uint seed, int length = 9)
    {
        var gameInfo = new GameInfo(gameRate, length, seed);
        InfoLogger.LogGame(gameInfo, gameRate);
    }
}
