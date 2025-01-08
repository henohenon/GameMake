using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

// アイテムの種類。数字付けてるのは追加や削除があってもシリアル化された既存の値が変化しないように
public enum ItemType
{
    Empty = -1,
    Flag = 0,
    SpeedUp = 1,
    SpeedDown = 2,
    ChangeFogEndDistanceUp = 3,
    ChangeFogEndDistanceDown = 4,

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
            case ItemType.ChangeFogEndDistanceUp://霧の視界綺麗に
                {

                    var value = RenderSettings.fogEndDistance;
                    //RenderSettings.fogEndDistance = value;
                    /*
                    LMotion.Create(0f, 8f, 2f) // 0fから10fまで2秒間でアニメーション
                    .WithEase(Ease.OutQuad)
                        .Bind(x  => value = x); // 任意の変数やフィールド、プロパティにバインド可能  
                    */
                    //RenderSettings.fogEndDistance = 8f;

                    LMotion.Create(0f, 8f, 2f) // 0fから10fまで2秒間でアニメーション
                        .Bind(x => RenderSettings.fogEndDistance = x); // 任意の変数やフィールド、プロパティにバインド可能

                    break;
                }
            case ItemType.ChangeFogEndDistanceDown://霧の視界綺麗に
                {
                    RenderSettings.fogEndDistance = 1f;
                    break;
                }
        }
    }
}
