using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class BlueItemTileController : TileController
{
    [SerializeField]
    private GameObject blueObject;
    [SerializeField] private TextMesh itemText;

    private string _itemIcon;

    public void SetItemIcon(string itemIcon)
    {
        _itemIcon = itemIcon;
        itemText.text = _itemIcon;
    }

    public override bool Open()
    {
        var baseResult = base.Open();
        if (baseResult)
        {
            blueObject.SetActive(true);
            itemText.gameObject.SetActive(true);
            Debug.Log(_itemIcon);
        }

        return baseResult;
    }
}
