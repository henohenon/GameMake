using Scriptable;
using Unity.VisualScripting;
using UnityEngine;

public static class InfoLogger
{
    public static void LogGame(GameInfo info, GameRateAsset rate)
    {
        LogMap(info.MapInfo, rate.mapRateAsset);
        LogItem(info.ItemInfo, rate.itemRateAsset);
    }
    
    public static void LogMap(MapInfo mapInfo,  MapRateAsset rateAsset)
    {
        Debug.Log("------------------------Map Width: " + mapInfo.MapLength.Width + ", Height: " + mapInfo.MapLength.Height);
        var mapText = "";
        for (var i = 0; i < mapInfo.MapLength.Height; i++)
        {
            for (int j = 0; j < mapInfo.MapLength.Width; j++)
            {
                var index = i * mapInfo.MapLength.Width + j;
                // タイルの種類を取得
                var tileType = rateAsset.tileRateInfos[mapInfo.Tiles[index]].tileType;
                // タイルの種類によって文字を変える
                switch (tileType)
                {
                    case TileType.Safety:
                        mapText += "S";
                        break;
                    case TileType.Bomb:
                        mapText += "B";
                        break;
                    case TileType.BlueItem:
                        mapText += "I";
                        break;
                    
                }
                
            }
            // 改行
            mapText += "\n";
        }
        
        Debug.Log(mapText);
    }
    public static void LogItem(ItemInfo itemInfo, ItemRateAsset rateAsset)
    {
        Debug.Log("----------------------Blue Item: ");
        // アイテムごとにループ
        foreach (var item in itemInfo.BlueResultItems)
        {
            // アイテムの種類を取得
            var type = item.itemType;
            // アイテムの説明を取得
            var description = rateAsset.blueItemRate.GetItemRateInfo(type).description;
            Debug.Log("Icon: " + item.itemIcon + ", Type: " + item.itemType + ", Description: " + description);
        }
        
        Debug.Log("-----------------------Yellow Item: ");
        // アイテムごとにループ
        for(var i = 0; i < itemInfo.YellowItems.Length; i++)
        {
            // アイテムの種類を取得
            var type = itemInfo.YellowItems[i];
            // アイテムの説明を取得
            var description = rateAsset.yellowItemRate.GetItemRateInfo(type).description;
            // そのアイテムになる計算結果のリストをテキストに
            var resultText = "";
            foreach (var result in itemInfo.YellowResultItems[i])
            {
                resultText += result + ", ";
            }
            Debug.Log("Result: " + resultText + "Type: " + type + ", Description: " + description);
        }
        // 黄色のパズルアイコンをその数と共に表示
        for(var i = 0; i < itemInfo.YellowPuzzleIcons.Length; i++)
        {
            // シャッフルしてもいいかも
            Debug.Log("Icon: " + itemInfo.YellowPuzzleIcons[i] + ", Number: " + i);
        }
        
        Debug.Log("-----------------------Purple Item: ");
        // アイテムごとにループ
        for(var i = 0; i < itemInfo.PurpleItems.Length; i++)
        {
            // アイテムの種類を取得
            var type = itemInfo.PurpleItems[i];
            // アイテムの説明を取得
            var description = rateAsset.purpleItemRate.GetItemRateInfo(type).description;
            // そのアイテムになる計算結果のリストをテキストに
            var resultText = "";
            foreach (var result in itemInfo.PurpleResultItems[i])
            {
                resultText += result + ", ";
            }
            Debug.Log("Result: " + resultText + "Type: " + type + ", Description: " + description);
        }
        // 紫色のパズルアイコンをその説明と共に表示
        for(var i = 0; i < rateAsset.purplePuzzleIcons.Length; i++)
        {
            Debug.Log("Icon: " + rateAsset.purplePuzzleIcons[i].icon + ", Description: " + rateAsset.purplePuzzleIcons[i].description);
        }
    }
}