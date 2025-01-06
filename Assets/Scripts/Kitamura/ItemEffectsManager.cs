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
    
    // アイテムの実行
    public void ExecItem(ItemType type)
    {
        // 種類ごとに処理を設定。
        // ここで制御できるようにしてあげることで、一括で速度アップの値の変更などができる
        switch (type)
        {
            case ItemType.Flag:
            {
                // 選択しているタイルの旗を切り替え
                if (_tileSelectManager.SelectingTile)
                {
                    _tileSelectManager.SelectingTile.ToggleFlag(flagPrefab);
                }
                break;
            }
            case ItemType.SpeedUp:
            {
                // 移動速度+2
                _playerController.AddMoveSpeedNumb(2f);
                break;
            }
            case ItemType.SpeedDown:
            {
                // 移動速度-2
                _playerController.AddMoveSpeedNumb(-2f);
                break;
            }
        }
    }
}
