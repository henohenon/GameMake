using RandomExtensions;
using Scriptable;
using UnityEngine;
using R3;
using Alchemy.Inspector;

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
    [SerializeField] private SoldierGameSoundManager soundManager;
    
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

        tilesManager.GameClear.Subscribe(_ =>
        {
            GameClear();
        });

        playerController.OnDamage.Subscribe(_ =>
        {
            GameOver();
        });
    }

    [Button]
    private void GameClear()
    {

        soundManager.PlayClearSound();
        playerController.ClearPose();
        playerController.SetCameraLock(false);
        soldierUIManager.SetPopupHidden(InPlayScreenType.GameClear,false);
        TimerManager._Running = false;
    }

    [Button]
    private void GameOver()
    {
        Debug.Log("GameOver");
        playerController.DeadPose();
        playerController.SetCameraLock(false);
        soldierUIManager.SetPopupHidden(InPlayScreenType.GameOver,false);
        TimerManager._Running = false;
        Debug.Log(TimerManager._Running);
    }
    
    private uint GenerateSeed()
    {
        // 最大5桁の乱数を生成
        return RandomEx.Shared.NextUInt(10000);
    }
}
