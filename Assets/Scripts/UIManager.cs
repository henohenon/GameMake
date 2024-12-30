using System.Collections;
using System.Collections.Generic;
using Scriptable;
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
        var tileMapsContainer = _root.Q<VisualElement>("tile-maps-container");
        tileMapsContainer.Clear();
    }
    
    public void WriteMap(GameRateAsset asset, MapInfo map, bool rightToLeft = false, bool bottomToTop = true)
    {
        // タイルコンテナーを生成
        var tileMapsContainer = _root.Q<VisualElement>("tile-maps-container");
        var tileContainer = new VisualElement();
        tileMapsContainer.Add(tileContainer);
        tileContainer.AddToClassList("tile-map");
        // マップのデータに沿ってタイルを生成
        for (int i = 0; i < map.Tiles.Length; i++)
        {
            // タイルのインデックスをフラグに応じて反転
            var tileIndex = i;
            if (rightToLeft)
            {
                tileIndex = MapTileCalc.GetInvertXId(tileIndex, map.Width);
            }
            if (bottomToTop)
            {
                tileIndex = MapTileCalc.GetInvertYId(tileIndex, map.Height);
            }
            
            // タイルのテンプレートを複製
            var tile = tileAsset.CloneTree();
            // idからタイルの名前を設定
            tile.name = $"Tile_{tileIndex}";
            // idのタイルの情報を取得
            var tileInfo = asset.tileRateInfos[map.Tiles[tileIndex]];
            // タイルコンテナーに追加
            tileContainer.Add(tile);
            // 爆弾なら赤にする
            if (tileInfo.tileType == TileType.Bomb)
            {
                tile.AddToClassList("red");
            }
        }
    }
}
