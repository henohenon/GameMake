using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardRateData", menuName = "CardRateData")]
public class CardRateAsset : ScriptableObject
{
    public EachCardRate[] cardRates;
    [NonSerialized] // シリアライズ(保存)しない
    private int _rateSum = -1;

    public GameObject GetRandomCard()
    {
        if (_rateSum == -1) // 初回のみrateSumを計算する(rateに-入れられたらバグる☆)
        {
            // 各レートを合算していく
            _rateSum = 0;
            foreach (var cardRate in cardRates)
            {
                _rateSum += cardRate.rate;
            }
        }

        // 0~最大値の間でランダムな値を取得
        var randomValue = UnityEngine.Random.Range(0, _rateSum);
        foreach (var card in cardRates)
        {
            // ランダムな値からレートを引いていく
            randomValue -= card.rate;
            // ランダムな値が0未満になったらそのカードを返す
            if (randomValue < 0)
            {
                return card.cardPrefab;
            }
        }
        return null;
        
    }
}

[Serializable] // クラスはserializableをつけるとインスペクターでいじれるようになる(シリアル化可能な変数で構成されている場合に限る)
public class EachCardRate
{
    public GameObject cardPrefab;
    public int rate;
}
