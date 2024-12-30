using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRateData", menuName = "ItemRateData")]
public class ItemRateAsset : ScriptableObject
{
    public BlueItemRate blueItemRate;
    public YellowItemRate yellowItemRate;
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
    public Sprite[] itemIcons;
    
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
    public Sprite[] numbIcons;
    // パズルの計算のアイコンの情報
    public YellowCalcIcon[] calcIcons; 
    
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
    public Sprite calcIcon;
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
    public Sprite calcIcon;
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
public class BluePuzzleInfo
{
    public BluePuzzleItem[] Items;
}

public class BluePuzzleItem
{
    public Sprite ItemIcon;
    public ItemType ItemType;
}

public enum YellowPuzzleCalcType
{
    Sum, // 和
    Difference, // 差
    Product, // 積
    Quotient, // 商
}

public class YellowPuzzleInfo
{
    public YellowPuzzleItem[] Items;
    public Dictionary<int, Sprite> NumbIcons;
    public Dictionary<YellowPuzzleCalcType, Sprite> CalcIcons;
}

public class YellowPuzzleItem
{
    public int[] ResultNumb;
    public ItemType ItemType;
}

public enum PurplePuzzleCalcType
{
    TryAngle, // 三角形
    Square, // 四角形
    Hexagon, // 六角形
}

public class PurplePuzzleInfo
{
    public Dictionary<int, ItemType> Items;
    public Dictionary<PurplePuzzleCalcType, Sprite> Icons;
    
    public int MaxNumb = 9;
    public int MinNumb = 1;
    public int MaxCalcIcons = 3;
    public int MinCalcIcons = 1;
}