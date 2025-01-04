using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CommanderUIManager : MonoBehaviour
{
    [SerializeField, AssetsOnly] private GameRateAsset rate;
    [SerializeField, AssetsOnly]
    private VisualTreeAsset tileAsset;// TODO: クラス指定のnew VisualElementで行けるようにしたい
    
    private UIDocument _document;
    private VisualElement _tileMapsContainer;
    private Label _detailsLabel;
    private TextField _idTextField;
    
    private void Start()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;
        

        var applyButton = root.Q<Button>("Apply");
        _idTextField = root.Q<TextField>("IdInputField");
        _detailsLabel = root.Q<Label>("Details");
        _tileMapsContainer = root.Q<VisualElement>("tile-maps-container");
        
        applyButton.clicked += RecreateView;

    }

    private const int mapLength = 9;
    private void RecreateView()
    {
        if (uint.TryParse(_idTextField.value, out uint result))
        {
            var gameInfo = new GameInfo(rate, mapLength, result);
            // マップのクリア
            _tileMapsContainer.Clear();
            // 本物+偽物*2のマップ配列を作成
            var maps = new []
            {
                gameInfo.MapInfo,
                new (rate.mapRateAsset.tileRateInfos, mapLength),
                new (rate.mapRateAsset.tileRateInfos, mapLength),
            }.AsEnumerable();
            // マップ配列シャッフル
            maps = maps.Shuffle().ToArray();
            // uiに書く
            foreach (var map in maps)
            {
                WriteMap(rate.mapRateAsset, map, RandomEx.Shared.NextBool(), RandomEx.Shared.NextBool());
            }
            // アイテム情報の表示
            _detailsLabel.text = GetItemInfoStr(gameInfo.ItemInfo, rate.itemRateAsset);
        }
        else
        {
            Debug.LogWarning("Invalid input. Reverting to previous value.");
        }
        
    }
    
    public void WriteMap(MapRateAsset rate, MapInfo map, bool rightToLeft = false, bool bottomToTop = true)
    {
        // タイルコンテナーを生成
        var tileContainer = new VisualElement();
        _tileMapsContainer.Add(tileContainer);
        tileContainer.AddToClassList("tile-map");
        // マップのデータに沿ってタイルを生成
        for (int i = 0; i < map.Tiles.Length; i++)
        {
            // タイルのインデックスをフラグに応じて反転
            var tileIndex = i;
            if (rightToLeft)
            {
                tileIndex = MapTileCalc.GetInvertXId(tileIndex, map.MapLength);
            }
            if (bottomToTop)
            {
                tileIndex = MapTileCalc.GetInvertYId(tileIndex, map.MapLength);
            }
            
            // タイルのテンプレートを複製
            var tile = tileAsset.CloneTree();
            // idからタイルの名前を設定
            tile.name = $"Tile_{tileIndex}";
            // idのタイルの情報を取得
            var tileInfo = rate.tileRateInfos[map.Tiles[tileIndex]];
            // タイルコンテナーに追加
            tileContainer.Add(tile);
            // 爆弾なら赤にする
            if (tileInfo.tileType == TileType.Bomb)
            {
                tile.AddToClassList("red");
            }
        }
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
