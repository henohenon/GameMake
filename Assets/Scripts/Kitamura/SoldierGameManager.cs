using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using UnityEngine;
using UnityEngine.Serialization;

public class SoldierGameManager : MonoBehaviour
{
    [SerializeField] private GameRateAsset gameRateAsset;
    [SerializeField] private int mapLength = 9;
    [SerializeField] private TilesManager tilesManager;
    
    private void Start()
    {
        // 乱数のシードを生成
        var seed = GenerateSeed();
        // ゲーム情報を初期化
        var gameInfo = new GameInfo(gameRateAsset, mapLength, seed);
        
        // ログ出力。TODO: UIにつなげる
        Debug.Log("Seed: " + seed);
        
        tilesManager.Generate3dMap(gameRateAsset.mapRateAsset, gameInfo);
    }
    
    private uint GenerateSeed()
    {
        // 最大5桁の乱数を生成
        return RandomEx.Shared.NextUInt(10000);
    }
}
