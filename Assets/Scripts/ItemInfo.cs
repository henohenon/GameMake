using System.Collections.Generic;
using System.Linq;
using RandomExtensions;
using RandomExtensions.Algorithms;
using RandomExtensions.Collections;
using RandomExtensions.Linq;
using Scriptable;
using UnityEditor.VersionControl;
using UnityEngine;

public class ItemInfo
{
    public readonly BlueResultItem[] BlueResultItems;
    
    public readonly ItemType[] yellowItems;
    public readonly List<int>[] yellowResultItems;
    public readonly string[] yellowPuzzleIcons;
    
    public readonly ItemType[] purpleItems;
    public readonly List<int>[] purpleResultItems;
    public PurplePuzzleIcon[] purplePuzzleIcons;

    public ItemInfo(ItemRateAsset asset, IRandom random)
    {
        // 青色アイテムの種類の重みつきリストを生成
        var blueItemArray = CreateWaitedArray(asset.blueItemRate.itemRateInfos);
        // 青色アイテムのアイコンのシャッフル配列の生成
        var blueIconArray = asset.blueItemRate.itemIcons.AsEnumerable();
        blueIconArray = blueIconArray.Shuffle(random).ToArray();
        
        // 青色アイテムの数をランダムに決定
        var blueItemCount = random.NextInt(asset.blueItemRate.minItemCount, asset.blueItemRate.maxItemCount);
        // 青色アイテムの配列を生成
        BlueResultItems = new BlueResultItem[blueItemCount];
        for(var i = 0; i < blueItemCount; i++)
        {
            // 青色アイテムの情報を取得
            var item = blueItemArray.GetItem(random);
            blueItemArray.Remove(item); // 重複を許可しないため、取得したアイテムをリストから削除
            var icon = blueIconArray.ElementAt(i);
            BlueResultItems[i] = new BlueResultItem(icon, item.itemType);

        }
        
        // 黄色アイテムの種類の重みつきリストを生成
        var yellowItemArray = CreateWaitedArray(asset.yellowItemRate.itemRateInfos);
        // 黄色アイテムの数をランダムに決定
        var yellowItemCount = asset.yellowItemRate.itemCount;
        // 今回のアイテムの重みつきリスト
        var yellowResultItemsArray = new WeightedList<int>();
        // 黄色アイテムの配列を生成
        yellowItems = new ItemType[yellowItemCount];
        for(var i = 0; i < yellowItemCount; i++)
        {
            var item = yellowItemArray.GetItem(random);
            yellowItemArray.Remove(item);
            // 今回使用するアイテムの種類を追加
            yellowItems[i] = item.itemType;
            // 今回のアイテムの重みつきリストに追加
            yellowResultItemsArray.Add(i, item.rate);
        }
        
        // 黄色アイテムの計算結果のシャッフル配列の生成
        yellowResultItems = new List<int>[yellowItemCount];
        var yellowResultEnumerable = Enumerable.Range(asset.yellowItemRate.MinResultNum, asset.yellowItemRate.MaxResultNum);
        var yellowResultArray = yellowResultEnumerable.Shuffle(random).ToArray();
        for(var i = 0; i < yellowResultArray.Length; i++)
        {
            var resultIndex = yellowResultArray[i];
            // ひとつづつアイテムが入るように
            if(i < yellowItemCount)
            {
                yellowResultItems[i] = new List<int> { resultIndex };
            }
            else
            {
                // 重みつきリストからランダムに取得
                var index = yellowResultItemsArray.GetItem(random);
                yellowResultItems[index].Add(resultIndex);
            }
        }
        // 黄色アイテムの記号のシャッフル配列の生成
        var yellowNumbIcons = asset.yellowItemRate.numbIcons.AsEnumerable(); // 元の配列が変更されないようコピーを作成。参照→値
        yellowNumbIcons = yellowNumbIcons.Shuffle(random).ToArray();
        // 黄色アイテムの記号の配列を生成
        var yellowNumbCount = asset.yellowItemRate.MaxNumbCount - asset.yellowItemRate.MinNumbCount;
        yellowPuzzleIcons = new string[yellowNumbCount];
        for(var i = 0; i < yellowNumbCount; i++)
        {
            yellowPuzzleIcons[i] = yellowNumbIcons.ElementAt(i);
        }
        
        // 紫色アイテムの種類の重みつきリストを生成
        var purpleItemArray = CreateWaitedArray(asset.purpleItemRate.itemRateInfos);
        // 紫色アイテムの数
        var purpleItemCount = asset.purpleItemRate.itemCount;
        // 紫色アイテムの配列を生成
        purpleItems = new ItemType[purpleItemCount];
        // 今回のアイテムの重みつきリスト
        var purpleResultItemsArray = new WeightedList<int>();
        for(var i = 0; i < purpleItemCount; i++)
        {
            var item = purpleItemArray.GetItem(random);
            purpleItemArray.Remove(item);
            purpleItems[i] = item.itemType;
            purpleResultItemsArray.Add(i, item.rate);
        }
        // 紫色アイテムの計算結果の数をランダムに設定
        purpleResultItems = new List<int>[purpleItemCount];
        var purpleResultEnumerable = Enumerable.Range(asset.purpleItemRate.minIconNumb, asset.purpleItemRate.maxIconNumb);
        var purpleResultArray = purpleResultEnumerable.Shuffle(random).ToArray();
        // 紫色アイテムの計算結果の数をランダムに設定
        for(var i = 0; i < purpleResultArray.Length; i++)
        {
            var resultIndex = purpleResultArray[i];
            if(i < purpleItemCount)
            {
                purpleResultItems[i] = new List<int> { resultIndex };
            }
            else
            {
                var index = purpleResultItemsArray.GetItem(random);
                purpleResultItems[index].Add(resultIndex);
            }
        }
    }
    
    private WeightedList<T> CreateWaitedArray<T> (T[] items) where T : RateItemBase
    {
        var weightList = new WeightedList<T>();
        foreach (var item in items)
        {
            weightList.Add(item, item.rate);
        }
        return weightList;
    }
}
