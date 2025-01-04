using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CommanderUIManager : MonoBehaviour
{
    private UIDocument _document;
    [SerializeField, AssetsOnly] private GameRateAsset rate;

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;

        var applyButton = root.Q<Button>("Apply");
        var idInputField = root.Q<TextField>("IdInputField");

        var detailText = root.Q<Label>("Details");

        applyButton.clicked += () =>
        {
            if (uint.TryParse(idInputField.value, out uint result))
            {
                var gameInfo = new GameInfo(rate, 9, result);
                detailText.text = GetItemInfoStr(gameInfo.ItemInfo, rate.itemRateAsset);
            }
            else
            {
                Debug.LogWarning("Invalid input. Reverting to previous value.");
            }
        };
    }

    private string GetItemInfoStr(ItemInfo info, ItemRateAsset rate)
    {
        string str = "";

        str += "----------------------Blue Item: \n";
        // アイテムごとにループ
        foreach (var item in info.BlueResultItems)
        {
            // アイテムの種類を取得
            var type = item.itemType;
            // アイテムの説明を取得
            var description = rate.blueItemRate.GetItemRateInfo(type).description;
            str += "Icon: " + item.itemIcon + ", Type: " + item.itemType + ", Description: " + description + "\n";
        }
        
        str += "-----------------------Yellow Item: " + "\n";
        // アイテムごとにループ
        for(var i = 0; i < info.YellowItems.Length; i++)
        {
            // アイテムの種類を取得
            var type = info.YellowItems[i];
            // アイテムの説明を取得
            var description = rate.yellowItemRate.GetItemRateInfo(type).description;
            // そのアイテムになる計算結果のリストをテキストに
            var resultText = "";
            foreach (var result in info.YellowResultItems[i])
            {
                resultText += result + ", ";
            }
            str += "Result: " + resultText + "Type: " + type + ", Description: " + description + "\n";
        }
        // 黄色のパズルアイコンをその数と共に表示
        for(var i = 0; i < info.YellowPuzzleIcons.Length; i++)
        {
            // シャッフルしてもいいかも
            str += "Icon: " + info.YellowPuzzleIcons[i] + ", Number: " + i + "\n";
        }
        
        str += "-----------------------Purple Item: " + "\n";
        // アイテムごとにループ
        for(var i = 0; i < info.PurpleItems.Length; i++)
        {
            // アイテムの種類を取得
            var type = info.PurpleItems[i];
            // アイテムの説明を取得
            var description = rate.purpleItemRate.GetItemRateInfo(type).description;
            // そのアイテムになる計算結果のリストをテキストに
            var resultText = "";
            foreach (var result in info.PurpleResultItems[i])
            {
                resultText += result + ", ";
            }
            str += "Result: " + resultText + "Type: " + type + ", Description: " + description + "\n";
        }
        // 紫色のパズルアイコンをその説明と共に表示
        for(var i = 0; i < rate.purplePuzzleIcons.Length; i++)
        {
            str += "Icon: " + rate.purplePuzzleIcons[i].icon + ", Description: " + rate.purplePuzzleIcons[i].description + "\n";
        }

        return str;
    }
}
