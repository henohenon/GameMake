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
    [SerializeField] private SoldierUIManager soldierUIManager;
    [SerializeField] private TilesManager tilesManager;
    
    private void Start()
    {
        // 乱数のシードを生成
        var seed = GenerateSeed();
        // ゲーム情報を初期化
        var gameInfo = new GameInfo(gameRateAsset, mapLength);
        
        // ログ出力。TODO: UIにつなげる
        Debug.Log("Seed: " + seed);
        InfoLogger.LogItem(gameInfo.ItemInfo, gameRateAsset.itemRateAsset);
        InfoLogger.LogMap(gameInfo.MapInfo, gameRateAsset);
        
        tilesManager.Generate3dMap(gameRateAsset.mapRateAsset, gameInfo);
        
        // 既存のタイルマップのUIをクリア
        soldierUIManager.ClearMaps();
        // 本物+偽物*2のマップ配列を作成
        var maps = new []
        {
            gameInfo.MapInfo,
            new (gameRateAsset.mapRateAsset.tileRateInfos, mapLength),
            new (gameRateAsset.mapRateAsset.tileRateInfos, mapLength),
        }.AsEnumerable();
        // マップ配列シャッフル
        maps = maps.Shuffle().ToArray();
        // uiに書く
        foreach (var map in maps)
        {
            soldierUIManager.WriteMap(gameRateAsset.mapRateAsset, map);
        }
    }
    
    private uint GenerateSeed()
    {
        // 最大5桁の乱数を生成
        return RandomEx.Shared.NextUInt(10000);
    }
}
