using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardRateData", menuName = "CardRateData")]
public class CardRateAsset : ScriptableObject
{
    public EachCardInfo[] cardInfos;
    public int _defaultCard = 0;
    [NonSerialized] // シリアライズ(保存)しない
    private int _rateSum = -1;
    
    public int GetRandomIndex()
    {
        if (_rateSum == -1) // 初回のみrateSumを計算する(rateに-入れられたらバグる☆)
        {
            // 各レートを合算していく
            _rateSum = 0;
            foreach (var cardRate in cardInfos)
            {
                _rateSum += cardRate.rate;
            }
        }

        // 0~最大値の間でランダムな値を取得
        var randomValue = UnityEngine.Random.Range(0, _rateSum);
        for (int i = 0; i < cardInfos.Length; i++)
        {
            // ランダムな値からレートを引いていく
            randomValue -= cardInfos[i].rate;
            // ランダムな値が0未満になったらそのカードを返す
            if (randomValue < 0)
            {
                return i;
            }
        }

        return -1;
    }
}

[Serializable] // クラスはserializableをつけるとインスペクターでいじれるようになる(シリアル化可能な変数で構成されている場合に限る)
public class EachCardInfo
{
    public CardController cardPrefab;
    public int rate;
    public CardType cardType;
}

public enum CardType
{
    Normal,
    Bomb,
}
