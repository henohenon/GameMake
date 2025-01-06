using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using UnityEngine;
using UnityEngine.Serialization;

// 他のスクリプトより後で実行。一括管理できないので乱立させていくとすごいことになるのであんまよくないけど一旦これで
[DefaultExecutionOrder(1)]
public class SoldierGameManager : MonoBehaviour
{
    [SerializeField] private GameRateAsset gameRateAsset;
    [SerializeField] private int mapLength = 9;
    [SerializeField] private TilesManager tilesManager;
    [SerializeField] private SoldierUIManager soldierUIManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TimerManager TimerManager;
    
    
    private void Start()
    {
        // 乱数のシードを生成
        var seed = GenerateSeed();
        Debug.Log("Seed: " + seed);
        // ゲーム情報を初期化
        var gameInfo = new GameInfo(gameRateAsset, mapLength, seed);
        
        // uiにシード値を表記
        soldierUIManager.SetShareID(seed);
        // ゲーム情報などからマップの生成
        tilesManager.Generate3dMap(gameRateAsset.mapRateAsset, gameInfo);
    }

    public void GameOver()
    {
        playerController.MovementPose();
        playerController.SetCameraLock(false);
        soldierUIManager.SetPopupHidden(InPlayScreenType.GameOver,false);
        TimerManager.OnStop();

    }
    
    private uint GenerateSeed()
    {
        // 最大5桁の乱数を生成
        return RandomEx.Shared.NextUInt(10000);
    }
}
