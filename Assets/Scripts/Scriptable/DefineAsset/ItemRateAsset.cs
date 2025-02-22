using System;
using System.Linq;
using UnityEngine;

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
        public PurplePuzzleIcon[] purplePuzzleIcons;
        

    }

    [Serializable]
    public class ItemRateInfo : RateItemBase
    {
        public ItemType itemType;
        public string description;
    }
    
    [Serializable]
    public abstract class ItemRateBase
    {
        public ItemRateInfo[] itemRateInfos;
        
        // Dictionaryにしたかったよ私だって。でもUnityのシリアライズができないんだよ。alchemyのシリアライズは挙動が怪しいんだよ。。
        public ItemRateInfo GetItemRateInfo(ItemType itemType)
        {
            return itemRateInfos.First(x => x.itemType == itemType);
        }
    }

    [Serializable]
    public class BlueItemRate: ItemRateBase
    {
        // パズルのアイコンの情報
        public ItemIcons[] itemIcons;

        [Header("アイテムの数")] public int maxItemCount = 7;
        public int minItemCount = 5;
    }

    [Serializable]
    public class ItemIcons
    {
        public Sprite itemIconBlack;
        public Sprite itemIconWhite;
    }
    
    [Serializable]
    public class YellowItemRate: ItemRateBase
    {
        // パズルのアイコンの情報
        public Sprite[] numbIcons;

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
    public class PurpleItemRate: ItemRateBase
    {
        // パズルのアイコンの情報
        public PurpleCalcIcon[] calcIcons;

        // 生成される答えの数
        public int itemCount = 3;

        [Header("パズル内のアイコンの数")]
        public int maxIconNumb = 5;
        public int minIconNumb = 3;
        [Header("計算の答えに使われる数字")]
        public readonly int MaxResultNum = 9;
        public readonly int MinResultNum = 1;
    }

    [Serializable]
    public class PurpleCalcIcon
    {
        public PurplePuzzleCalcType calcType;
        public string calcIcon;
    }

    [Serializable]
    public class BlueResultItem
    {
        public ItemIcons itemIcon;
        public ItemType itemType;

        public BlueResultItem(ItemIcons itemIcons, ItemType itemType)
        {
            this.itemIcon = itemIcons;
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
        public string description;
    }
}