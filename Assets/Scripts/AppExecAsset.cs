using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

// ランダムに選ばれる実行データを保持するクラス
[CreateAssetMenu(fileName = "AppExecData", menuName = "AppExecData")]
public class AppExecAsset : ScriptableObject
{
    public GameRateAsset gameRate;
    [ReadOnly]
    public SquareTileMap[] execMaps;
    [ReadOnly]
    public ItemPuzzleInfo[] execPuzzles;
    
    // ランダムなマップを生成
    [Button]
    public void CreateRandomMaps(int mapLength, int mapCount)
    {
        execMaps = new SquareTileMap[mapCount];
        for (var i = 0; i < mapCount; i++)
        {
            execMaps[i] = new SquareTileMap(gameRate, mapLength, mapLength);
        }
    }
    
    [Button]
    public void CreateRandomPuzzles(int puzzleCount)
    {
        execPuzzles = new ItemPuzzleInfo[puzzleCount];
        for (var i = 0; i < puzzleCount; i++)
        {
            execPuzzles[i] = new ItemPuzzleInfo(gameRate.itemRateAsset);
        }
    }
}
