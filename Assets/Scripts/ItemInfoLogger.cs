using Scriptable;
using Unity.VisualScripting;
using UnityEngine;

public static class ItemInfoLogger
{
    public static void LogItem(ItemInfo itemInfo, ItemRateAsset rateAsset)
    {
        Debug.Log("----------------------Blue Item: ");
        foreach (var item in itemInfo.BlueResultItems)
        {
            var type = item.itemType;
            var description = rateAsset.blueItemRate.GetItemRateInfo(type).description;
            Debug.Log("Icon: " + item.itemIcon + ", Type: " + item.itemType + ", Description: " + description);
        }
        
        Debug.Log("-----------------------Yellow Item: ");
        for(var i = 0; i < itemInfo.YellowItems.Length; i++)
        {
            var type = itemInfo.YellowItems[i];
            var description = rateAsset.yellowItemRate.GetItemRateInfo(type).description;
            var resultText = "";
            foreach (var result in itemInfo.YellowResultItems[i])
            {
                resultText += result + ", ";
            }
            Debug.Log("Result: " + resultText + "Type: " + type + ", Description: " + description);
        }
        for(var i = 0; i < itemInfo.YellowPuzzleIcons.Length; i++)
        {
            Debug.Log("Icon: " + itemInfo.YellowPuzzleIcons[i] + ", Number: " + i);
        }
        
        Debug.Log("-----------------------Purple Item: ");
        for(var i = 0; i < itemInfo.PurpleItems.Length; i++)
        {
            var type = itemInfo.PurpleItems[i];
            var description = rateAsset.purpleItemRate.GetItemRateInfo(type).description;
            var resultText = "";
            foreach (var result in itemInfo.PurpleResultItems[i])
            {
                resultText += result + ", ";
            }
            Debug.Log("Result: " + resultText + "Type: " + type + ", Description: " + description);
        }
        
        for(var i = 0; i < rateAsset.purplePuzzleIcons.Length; i++)
        {
            Debug.Log("Icon: " + rateAsset.purplePuzzleIcons[i].icon + ", Description: " + rateAsset.purplePuzzleIcons[i].description);
        }
    }
}