using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;

// アイテムの種類。数字付けてるのは追加や削除があってもシリアル化された既存の値が変化しないように
public enum ItemType
{
    Empty = -1,
    Flag = 0,
    SpeedUp = 1,
    SpeedDown = 2,
}

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
