using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;

public class ItemEffectsManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TileSelectManager _tileSelectManager;
    [SerializeField, AssetsOnly] private GameObject flagPrefab;
    
    public void ExecItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Flag:
            {
                if (_tileSelectManager.SelectingTile)
                {
                    _tileSelectManager.SelectingTile.Flag(flagPrefab);
                }
                break;
            }
            case ItemType.SpeedUp:
            {
                _playerController.AddMoveSpeedNumb(2f);
                break;
            }
            case ItemType.SpeedDown:
            {
                _playerController.AddMoveSpeedNumb(-2f);
                break;
            }
        }
    }
}
