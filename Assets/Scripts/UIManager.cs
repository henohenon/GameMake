using System.Collections;
using System.Collections.Generic;
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

    public void WriteMap(SquareMap map)
    {
        var tileTemplate = _root.Q<VisualElement>("TileContainer");
        tileTemplate.Clear();
        for (int i = 0; i < map.Map.Length; i++)
        {
            var tile = tileAsset.CloneTree();
            tile.name = $"Tile_{i}";
            Debug.Log(map.Asset.cardInfos[map.Map[i]].cardType);
            var tileInfo = map.Asset.cardInfos[map.Map[i]];
            tileTemplate.Add(tile);
            if (tileInfo.cardType == CardType.Bomb)
            {
                tile.AddToClassList("red");
            }
        }
    }
}
