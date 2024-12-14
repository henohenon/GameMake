using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;
    [SerializeField]
    private VisualTreeAsset tileAsset;
    private VisualElement _root;

    private void Awake()
    {
        _root = uiDocument.rootVisualElement;
    }
    
    public void ClearMaps()
    {
        var containers = _root.Query<VisualElement>().Class("tile-container").ToList();
        foreach (var container in containers)
        {
            container.RemoveFromHierarchy();
        }
    }
    
    public void WriteMap(SquareTileMap tileMap, bool rightToLeft = false, bool bottomToTop = true)
    {
        // タイルコンテナーを生成
        var tileContainer = new VisualElement();
        _root.Add(tileContainer);
        tileContainer.AddToClassList("tile-container");
        // マップのデータに沿ってタイルを生成
        for (int i = 0; i < tileMap.Map.Length; i++)
        {
            // タイルのインデックスをフラグに応じて反転
            var cardIndex = i;
            if (rightToLeft)
            {
                cardIndex = MapTileCalc.GetInvertXId(cardIndex, tileMap.Width);
            }
            if (bottomToTop)
            {
                cardIndex = MapTileCalc.GetInvertYId(cardIndex, tileMap.Height);
            }
            
            // タイルのテンプレートを複製
            var tile = tileAsset.CloneTree();
            // idからタイルの名前を設定
            tile.name = $"Tile_{cardIndex}";
            // idのタイルの情報を取得
            var tileInfo = tileMap.Asset.cardInfos[tileMap.Map[cardIndex]];
            // タイルコンテナーに追加
            tileContainer.Add(tile);
            // 爆弾なら赤にする
            if (tileInfo.cardType == CardType.Bomb)
            {
                tile.AddToClassList("red");
            }
        }
    }
}
