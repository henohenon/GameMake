using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameRateData", menuName = "GameRateData")]
public class GameRateAsset : ScriptableObject
{
    public EachTileInfo[] tileInfos;
    public int defaultTile = 0;
    public ItemRateAsset itemRateAsset;
    
    [NonSerialized] // シリアライズ(保存)しない
    private int _rateSum = -1;
    
    public int GetRandomIndex()
    {
        if (_rateSum == -1) // 初回のみrateSumを計算する(rateに-入れられたらバグる☆)
        {
            // 各レートを合算していく
            _rateSum = 0;
            foreach (var tileInfo in tileInfos)
            {
                _rateSum += tileInfo.rate;
            }
        }

        // 0~最大値の間でランダムな値を取得
        var randomValue = UnityEngine.Random.Range(0, _rateSum);
        for (int i = 0; i < tileInfos.Length; i++)
        {
            // ランダムな値からレートを引いていく
            randomValue -= tileInfos[i].rate;
            // ランダムな値が0未満になったらそのタイルを返す
            if (randomValue < 0)
            {
                return i;
            }
        }

        return -1;
    }
}


[Serializable] // クラスはserializableをつけるとインスペクターでいじれるようになる(シリアル化可能な変数で構成されている場合に限る)
public class EachTileInfo
{
    public TileController tilePrefab;
    public int rate;
    public TileType tileType;
}

public enum TileType
{
    First,
    Normal,
    Item,
    Bomb,
}