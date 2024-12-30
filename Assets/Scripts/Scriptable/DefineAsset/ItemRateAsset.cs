using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アイテムのレートを統合して持つクラス
[CreateAssetMenu(fileName = "ItemRateData", menuName = "ItemRateData")]
public class ItemRateAsset : ScriptableObject
{
    public BlueItemRate blueItemRate;
    public YellowItemRate yellowItemRate;
    public YellowPuzzleCalcIcon[] yellowCalcIcons;
    public PurpleItemRate purpleItemRate;
    
    
    public TileController blueItemTilePrefab;
    public TileController yellowItemTilePrefab;
    public TileController purpleItemTilePrefab;
}

[Serializable]
public class ItemRateInfo
{
    public int rate;
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
    
    // 生成されるパズルの数
    public int maxItemNum = 5;
    public int minItemNum = 3;
}

[Serializable]
public class YellowItemRate
{
    // アイテムの効果の情報
    public ItemRateInfo[] itemRateInfos;
    // パズルのアイコンの情報
    public string[] numbIcons;
    
    // 生成される答えの数
    public int maxResultNum = 3;
    public int minResultNum = 1;
    
    // 生成される計算の数
    public int maxCalcNumb = 3;
    public readonly int MinCalcNumb = 2; // 最小は2で固定
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
    public int maxResultNum = 3;
    public int minResultNum = 1;
    
    // 生成される計算の数
    public int maxIconNumb = 3;
    public int minIconNumb = 0;
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

[Serializable]
public class ItemPuzzleInfo
{
    [Header("Blue Puzzle")]
    public BluePuzzleItem[] bluePuzzleItems;
    [Header("Yellow Puzzle")]
    public YellowPuzzleItem[] yellowPuzzleItems;
    public string[] yellowNumbIcons;
    [Header("Purple Puzzle")]
    public ItemType[] purplePuzzleItems;
    public PurplePuzzleCalcIcon[] purpleCalcIcons;

    public ItemPuzzleInfo(ItemRateAsset itemRate)
    {
        bluePuzzleItems = new BluePuzzleItem[itemRate.blueItemRate.maxItemNum];
        for (var i = 0; i < bluePuzzleItems.Length; i++)
        {
            bluePuzzleItems[i] = new BluePuzzleItem(itemRate.blueItemRate.itemIcons[i], itemRate.blueItemRate.itemRateInfos[i].itemType);
        }
        
        yellowPuzzleItems = new YellowPuzzleItem[itemRate.yellowItemRate.maxResultNum];
        for (var i = 0; i < yellowPuzzleItems.Length; i++)
        {
            yellowPuzzleItems[i] = new YellowPuzzleItem(new int[itemRate.yellowItemRate.maxCalcNumb], itemRate.yellowItemRate.itemRateInfos[i].itemType);
        }
        
        purplePuzzleItems = new ItemType[itemRate.purpleItemRate.maxResultNum];
        for (var i = 0; i < purplePuzzleItems.Length; i++)
        {
            purplePuzzleItems[i] = itemRate.purpleItemRate.itemRateInfos[i].itemType;
        }
        
        purpleCalcIcons = new PurplePuzzleCalcIcon[itemRate.purpleItemRate.maxIconNumb];
        for (var i = 0; i < purpleCalcIcons.Length; i++)
        {
            purpleCalcIcons[i] = new PurplePuzzleCalcIcon(itemRate.purpleItemRate.calcIcons[i].calcType, itemRate.purpleItemRate.calcIcons[i].calcIcon);
        }
    }
}

[Serializable]
public class BluePuzzleItem
{
    public string itemIcon;
    public ItemType itemType;
    
    public BluePuzzleItem(string itemIcon, ItemType itemType)
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
public class YellowPuzzleCalcIcon
{
    public YellowPuzzleCalcType calcType;
    public String calcText;
    
    public YellowPuzzleCalcIcon(YellowPuzzleCalcType calcType, String calcText)
    {
        this.calcType = calcType;
        this.calcText = calcText;
    }
}

[Serializable]
public class YellowPuzzleItem
{
    public int[] resultNumb;
    public ItemType itemType;
    
    public YellowPuzzleItem(int[] resultNumb, ItemType itemType)
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
public class PurplePuzzleCalcIcon
{
    public PurplePuzzleCalcType calcType;
    public string icon;
    
    public PurplePuzzleCalcIcon(PurplePuzzleCalcType calcType, string icon)
    {
        this.calcType = calcType;
        this.icon = icon;
    }
}