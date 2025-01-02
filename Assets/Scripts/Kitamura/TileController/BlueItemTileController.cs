using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class BlueItemTileController : TileController
{
    [SerializeField]
    private GameObject blueObject;

    private string _itemIcon;

    public void SetItemIcon(string itemIcon)
    {
        this._itemIcon = itemIcon;
    }

    public override bool Open()
    {
        var baseResult = base.Open();
        if (baseResult)
        {
            blueObject.SetActive(true);
            Debug.Log(_itemIcon);
        }

        return baseResult;
    }
}
