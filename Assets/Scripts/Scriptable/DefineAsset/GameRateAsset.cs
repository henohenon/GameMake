using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using RandomExtensions.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable
{
    // アイテムとマップのレートを持ち、ゲームのランダム要素のレートを統合して保有するクラス
    [CreateAssetMenu(fileName = "GameRateData", menuName = "GameRateData")]
    public class GameRateAsset : ScriptableObject
    {
        [InlineEditor,AssetsOnly] public MapRateAsset mapRateAsset;
        [InlineEditor,AssetsOnly] public ItemRateAsset itemRateAsset;
    }

    
    /*
     * RandomExを生かしたint rateを用いた重みつきリストのラッパークラス達
     * 本来であれば、WeightList<T>をAlchemySerializeできればこんなものはいらないのだが
     * AlchemySerializeはScriptableだと安定しないため、今回はこのような形になっている
     */
    [Serializable]
    public abstract class RateItemBase
    {
        public int rate;
    }

    // RateItemBase[]を内部的にWeightedList<T>に変換、ラップするクラス
    public class RandomRateList<T> where T : RateItemBase
    {
        private readonly T[] _items;
        private readonly WeightedList<T> _weightedList;
        private readonly bool _duplication;

        public RandomRateList(T[] items, bool duplication = false)
        {
            _items = items;
            // 重みつきリストに変換
            _weightedList = new WeightedList<T>();
            foreach (var item in items)
            {
                _weightedList.Add(item, item.rate);
            }

            _duplication = duplication;
        }

        public T Get()
        {
            var item = _weightedList.GetItem();
            // 重複を許可しない場合はリストから削除
            if (!_duplication)
            {
                _weightedList.Remove(item);
            }

            return item;
        }
    }
}