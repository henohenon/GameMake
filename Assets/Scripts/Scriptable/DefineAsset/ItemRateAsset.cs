using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RandomExtensions;
using RandomExtensions.Collections;
using RandomExtensions.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable
{

// アイテムのレートを統合して持つクラス
    [CreateAssetMenu(fileName = "ItemRateData", menuName = "ItemRateData")]
    public class ItemRateAsset : ScriptableObject
    {
        public BlueItemRate blueItemRate;
        public YellowItemRate yellowItemRate;
        public YellowPuzzleIcon[] yellowPuzzleIcons;
        public PurpleItemRate purpleItemRate;

        public TileController blueItemTilePrefab;
        public TileController yellowItemTilePrefab;
        public TileController purpleItemTilePrefab;
    }

    [Serializable]
    public class ItemRateInfo : RateItemBase
    {
        public ItemType itemType;
        public string itemText;
    }

    [Serializable]
    public class BlueItemRate
    {
        // アイテムの効果の情報
        public ItemRateInfo[] itemRateInfos;

        // パズルのアイコンの情報
        public string[] itemIcons;

        [Header("アイテムの数")] public int maxItemCount = 7;
        public int minItemCount = 5;
    }

    [Serializable]
    public class YellowItemRate
    {
        // アイテムの効果の情報
        public ItemRateInfo[] itemRateInfos;

        // パズルのアイコンの情報
        public string[] numbIcons;

        [Header("アイテムの数")] public int itemCount = 3;

        [Header("計算に使われる項の数")] public int maxCalcNumb = 3;
        public readonly int MinCalcNumb = 2; // 最小は2で固定

        [Header("計算に使われる数字の数")] public readonly int MaxNumbCount = 9;
        public readonly int MinNumbCount = 0;

        [Header("計算の答えに使われる数字")] public readonly int MaxResultNum = 9;
        public readonly int MinResultNum = 1;
    }

    [Serializable]
    public class YellowCalcIcon
    {
        public YellowPuzzleCalcType calcType;
        public String calcText;
    }

    [Serializable]
    public class PurpleItemRate
    {
        // アイテムの効果の情報
        public ItemRateInfo[] itemRateInfos;

        // パズルのアイコンの情報
        public PurpleCalcIcon[] calcIcons;

        // 生成される答えの数
        public int itemCount = 3;

        [Header("パズル内のアイコンの数")] public int maxIconNumb = 5;
        public int minIconNumb = 3;
    }

    [Serializable]
    public class PurpleCalcIcon
    {
        public PurplePuzzleCalcType calcType;
        public string calcIcon;
    }

    public enum ItemTier
    {
        Blue,
        Yellow,
        Purple,
    }

    public enum ItemType
    {
        SpeedUp,
        SpeedDown,
    }

/*
 * 以下レートから生成された実行されているゲーム内でのパズル/アイテムの情報
 */

// 全パズルの情報を保持するクラス
    [Serializable]
    public class ItemPuzzleInfo
    {
        [Header("Blue Puzzle")] public SingleResultItem[] blueResultItems;
        [Header("Yellow Puzzle")] public MultiResultsItem[] yellowResultItems;
        public string[] yellowPuzzleIcons;
        [Header("Purple Puzzle")] public MultiResultsItem[] purpleResultItems;
        public PurplePuzzleIcon[] purplePuzzleIcons;

        public ItemPuzzleInfo(ItemRateAsset r)
        {
            // 青のパズルの数をランダムに設定
            var bluePuzzleCount = RandomEx.Shared.NextInt(r.blueItemRate.minItemCount, r.blueItemRate.maxItemCount);
            blueResultItems = new SingleResultItem[bluePuzzleCount];
            // アイテムをレートに基づいてランダムに取得できるやつ
            var blueItemList = new RandomRateList<ItemRateInfo>(r.blueItemRate.itemRateInfos);
            // アイテムのアイコン配列をシャッフル
            var blueItemIcons = r.blueItemRate.itemIcons.ToArray(); // 元の配列が変更されないようコピーを作成。参照→値
            RandomEx.Shared.Shuffle(blueItemIcons);

            // アイテムの情報をランダムに設定
            for (var i = 0; i < blueResultItems.Length; i++)
            {
                var randomItem = blueItemList.Get();
                blueResultItems[i] = new SingleResultItem(blueItemIcons[i], randomItem.itemType);
            }

            // 黄色のアイテム効果と計算結果の数をランダムに設定
            yellowResultItems = GetMultiResultsItem(r.yellowItemRate.itemRateInfos, r.yellowItemRate.itemCount,
                r.yellowItemRate.MinResultNum, r.yellowItemRate.MaxResultNum);
            // アイコン配列の初期化
            yellowPuzzleIcons = new string[r.yellowItemRate.MaxNumbCount - r.yellowItemRate.MinNumbCount];
            // アイテムのアイコン配列をシャッフル
            var yellowNumbIcons = r.yellowItemRate.numbIcons.ToArray(); // 元の配列が変更されないようコピーを作成。参照→値
            RandomEx.Shared.Shuffle(yellowNumbIcons);
            // アイコンアイコンをランダムに設定
            for (var i = 0; i < yellowPuzzleIcons.Length; i++)
            {
                yellowPuzzleIcons[i] = yellowNumbIcons[i];
            }

            // 黄色のアイテム効果と計算結果の数をランダムに設定
            purpleResultItems = GetMultiResultsItem(r.purpleItemRate.itemRateInfos, r.purpleItemRate.itemCount,
                r.purpleItemRate.minIconNumb, r.purpleItemRate.maxIconNumb);
            // 紫のパズルのアイテムの数をランダムに設定
            purplePuzzleIcons = new PurplePuzzleIcon[r.purpleItemRate.calcIcons.Length];
            for (int i = 0; i < purplePuzzleIcons.Length; i++)
            {
                purplePuzzleIcons[i].icon = r.purpleItemRate.calcIcons[i].calcIcon;
                purplePuzzleIcons[i].calcType = r.purpleItemRate.calcIcons[i].calcType;
                purplePuzzleIcons[i].number =
                    RandomEx.Shared.NextInt(r.purpleItemRate.minIconNumb, r.purpleItemRate.maxIconNumb);
            }
        }

        private MultiResultsItem[] GetMultiResultsItem(ItemRateInfo[] rateInfos, int itemCount, int minResultNum,
            int maxResultNum)
        {
            // 結果の定義
            var result = new MultiResultsItem[itemCount];
            // アイテムをレートに基づいてランダムに取得できるやつ
            var itemRatesList = new RandomRateList<ItemRateInfo>(rateInfos);
            // ランダムな計算結果の数字の配列
            var yellowResultNumbs = Enumerable.Range(minResultNum, maxResultNum);
            yellowPuzzleIcons.Shuffle();
            yellowResultNumbs = yellowResultNumbs.ToList();

            // アイテムの情報を設定していく
            for (var i = 0; i < itemCount; i++)
            {
                // アイテム効果をランダムに設定
                result[i].itemType = itemRatesList.Get().itemType;

                // 計算結果の数の合計を設定
                var resultCount = yellowResultNumbs.Count();
                // 最後の処理でないなら
                if (i + i < result.Length)
                {
                    // 最大数を 計算結果の数の合計 - アイテム効果の数 + インデックス に設定
                    var maxResultCount = resultCount - result.Length + i;
                    // 1~最大数の間でランダムな数を取得
                    resultCount = RandomEx.Shared.NextInt(1, maxResultCount);
                }

                // 計算結果の数の合計から取得した数だけ取得
                var resultNumbs = yellowResultNumbs.Take(resultCount).ToArray();
                yellowResultNumbs = yellowResultNumbs.Skip(resultCount).ToArray();
                result[i].resultNumb = resultNumbs;
            }

            return result;
        }
    }

    [Serializable]
    public class SingleResultItem
    {
        public string itemIcon;
        public ItemType itemType;

        public SingleResultItem(string itemIcon, ItemType itemType)
        {
            this.itemIcon = itemIcon;
            this.itemType = itemType;
        }
    }

    public enum YellowPuzzleCalcType
    {
        Sum, // 和
        Difference, // 差
        Product, // 積
        Quotient, // 商
    }

    [Serializable]
    public class YellowPuzzleIcon
    {
        public YellowPuzzleCalcType calcType;
        public String calcText;

        public YellowPuzzleIcon(YellowPuzzleCalcType calcType, String calcText)
        {
            this.calcType = calcType;
            this.calcText = calcText;
        }
    }

    [Serializable]
    public class MultiResultsItem
    {
        public int[] resultNumb;
        public ItemType itemType;

        public MultiResultsItem(int[] resultNumb, ItemType itemType)
        {
            this.resultNumb = resultNumb;
            this.itemType = itemType;
        }
    }

    public enum PurplePuzzleCalcType
    {
        TryAngle, // 三角形
        Square, // 四角形
        Hexagon, // 六角形
    }

    [Serializable]
    public class PurplePuzzleIcon
    {
        public PurplePuzzleCalcType calcType;
        public string icon;
        public int number;

        public PurplePuzzleIcon(PurplePuzzleCalcType calcType, string icon, int number)
        {
            this.calcType = calcType;
            this.icon = icon;
            this.number = number;
        }
    }
}