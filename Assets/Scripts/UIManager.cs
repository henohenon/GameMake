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

    public void WriteMap(SquareMap map, bool leftToRight = true, bool topToBottom = false)
    {
        var tileTemplate = _root.Q<VisualElement>("TileContainer");
        tileTemplate.Clear();
        for (int i = 0; i < map.Map.Length; i++)
        {
            var tile = tileAsset.CloneTree();
            tile.name = $"Tile_{i}";
            var cardIndex = i;
            if (!leftToRight)
            {
                cardIndex = MapCalculation.GetInvertXId(cardIndex, map.Width);
            }
            if (!topToBottom)
            {
                cardIndex = MapCalculation.GetInvertYId(cardIndex, map.Height);
            }
            var tileInfo = map.Asset.cardInfos[map.Map[cardIndex]];
            tileTemplate.Add(tile);
            if (tileInfo.cardType == CardType.Bomb)
            {
                tile.AddToClassList("red");
            }
        }
    }
}
