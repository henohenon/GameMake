using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using RandomExtensions;
using RandomExtensions.Linq;
using Scriptable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CmdController : MonoBehaviour
{
    [SerializeField] private GameRateAsset gameRateAsset;
    [SerializeField] private VisualTreeAsset helperTreeView;
    [SerializeField] private InputActionReference execInput;
    [SerializeField] private InputActionReference anyInput;
    [SerializeField] private InputActionReference previousInput;
    [SerializeField] private InputActionReference nextInput;
    [SerializeField] private InputActionReference closeInput;
    private TextField _currentTextField = null;
    private VisualElement _root;
    
    // Start is called before the first frame update
    private void Start()
    {
        execInput.action.started += ctx => Exec();
        execInput.action.Enable();
        closeInput.action.started += ctx => ReturnToTitle();
        closeInput.action.Enable();
        
        var uiDocument = GetComponent<UIDocument>();
        var rootElement = uiDocument.rootVisualElement;

        _root = rootElement.Q<VisualElement>("unity-content-container");
        
        _root.Add(helperTreeView.CloneTree());
        
        CreateNewLine();
    }

    private async void Exec()
    {
        var input = _currentTextField.text;
        await ExecuteCommand(input);
        
        CreateNewLine();
    }
    
    // 正規表現でecho "any text"形式を確認
    private const string MscPattern = @"^msc\s+(.+)$";
    private const string EchoPattern = @"^echo\s+""(.*?)""$";
    private const string CdPattern = @"^cd\s+(.+)$";
    private const string LsPattern = @"^ls(\s+.+)$";
    private const string CatPattern = @"^cat\s+(.+)$";

    private async UniTask ExecuteCommand(string command)
    {
        var mscMatch = Regex.Match(command, MscPattern);
        if (mscMatch.Success)
        {
            var mscText = mscMatch.Groups[1].Value;
            if (uint.TryParse(mscText, out uint result))
            {
                
                await RecreateView(result);
            }
            else
            {
                CreateLog(mscText + " is not a valid number");
            }
            
            return;
        }

        if (command == "ctr+c")
        {
            ReturnToTitle();
        }
        
        CreateLog("command not found");
    }

    private void CreateLog(string text)
    {
        var logText = new Label(text);
        _root.Add(logText);
    }
    
    private void CreateNewLine()
    {
        if (_currentTextField != null)
        {
            _currentTextField.isReadOnly = true;
        }
        
        var line = new TextField();
        _root.Add(line);
        _currentTextField = line;
        
        line.label = "multisweeper@commander:~/opt/msc $";
        
        line.Focus();
        line.Focus();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_currentTextField != null)
        {
            Debug.Log(_currentTextField.cursorPosition);
        }

    }

    private void ReturnToTitle(){
        SceneManager.LoadScene("TitleScene");
    }

    private const int mapLength = 9;
    private async UniTask RecreateView(uint seed)
    {
        // データの生成
        var gameInfo = new GameInfo(gameRateAsset, mapLength, seed);
        var moreRandom = new Xoshiro256StarStarRandom(seed+1);
        // 本物+偽物*2のマップ配列を作成
        var maps = new []
        {
            gameInfo.MapInfo,
            new (gameRateAsset.mapRateAsset.tileRateInfos, moreRandom, mapLength),
            new (gameRateAsset.mapRateAsset.tileRateInfos, moreRandom, mapLength),
        }.AsEnumerable();
        // マップ配列シャッフル
        maps = maps.Shuffle().ToArray();
     
        var generateRoot = new VisualElement();
        _root.Add(generateRoot);
        var loadingText = new Label("...loading");
        generateRoot.Add(loadingText);
        await UniTask.WaitForSeconds(0.5f);
        loadingText.text = "...searching";
        await UniTask.WaitForSeconds(0.75f);
        
        generateRoot.Remove(loadingText);


        var mapText = new Label("三つのうち一つがホンモノのマップです");
        _root.Add(mapText);
        
        var tileContainer = new VisualElement();
        _root.Add(tileContainer);
        
        // uiに書く
        foreach (var map in maps)
        {
            WriteMap(tileContainer, gameRateAsset.mapRateAsset, map);
        }
        
        var itemText = new Label("アイテムアイコン: 効果");
        _root.Add(itemText);
        // アイテム情報の表示
        foreach (var blueResultItem in gameInfo.ItemInfo.BlueResultItems)
        {
            var itemRow = new VisualElement();
            itemRow.AddToClassList("itemRow");
            
            var itemIcon = new VisualElement();
            itemIcon.style.backgroundImage = blueResultItem.itemIcon.texture;
            itemIcon.AddToClassList("itemIcon");
            var itemRateInfo = gameRateAsset.itemRateAsset.blueItemRate.GetItemRateInfo(blueResultItem.itemType);
            Debug.Log(itemRateInfo.description);
            var itemInfo = new Label(itemRateInfo.description);
            itemInfo.AddToClassList("itemInfo");
            itemRow.Add(itemIcon);
            itemRow.Add(itemInfo);
            
            _root.Add(itemRow);
        }
    }
    
    public void WriteMap(VisualElement mapContainer, MapRateAsset rate, MapInfo map, bool rightToLeft = false, bool bottomToTop = true)
    {
        // タイルコンテナーを生成
        var tileContainer = new VisualElement();
        mapContainer.Add(tileContainer);
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
            var tile = new VisualElement();
            tile.AddToClassList("tile");
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
    
    private void OnDisable()
    {
        execInput.action.Disable();
        closeInput.action.Disable();
    }
}
