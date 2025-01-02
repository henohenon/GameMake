using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class BlueItemTileController : TileController
{
    [SerializeField]
    private GameObject blueObject;


    public override void Open()
    {
        base.Open();
        blueObject.SetActive(true);
    }
}
