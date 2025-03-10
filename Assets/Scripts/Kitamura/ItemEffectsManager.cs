using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using Scriptable;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.Experimental.GlobalIllumination;
using RenderSettings = UnityEngine.RenderSettings;
using Unity.VisualScripting;
using System.Linq;

// アイテムの種類。数字付けてるのは追加や削除があってもシリアル化された既存の値が変化しないように
public enum ItemType
{
    Empty = -1,
    Flag = 0,
    SpeedUp = 1,
    SpeedDown = 2,
    LightUp = 3,
    LightDown = 4,
    OpenFrontLine = 5,
    RandomMovement = 6,
    AddOpenPosition = 7,
    ViewBombNumb = 8,
    changefootsteps = 9,
}

public class ItemEffectsManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TileSelectManager _tileSelectManager;
    [SerializeField] private TilesManager tilesManager;
    [SerializeField, AssetsOnly] private FlagController flagPrefab;
    [SerializeField] private Light light;
    [SerializeField] public AudioClip _changestepsAudioClip;
    [SerializeField] public AudioClip noneitem;//アイテムがない使用時の音
    [SerializeField] public AudioClip useitem;//アイテム使用時の音

    //public TileSelectManager tileSelectManager;
    public AudioSource _audioSource;//アイテム使用時の音

    // private LightDistanceType _currentLightDistanceType = LightDistanceType.Normal;

    private static readonly Vector2[] OpenPositions = new Vector2[]
    {
        new Vector2(1, 1),
        new Vector2(-1, 1),
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(1, -1),
        new Vector2(-1, -1),
    };

    private void Start()
    {
        // _lightDistances[_currentLightDistanceType].ApplyValues(light);
        //_stepsAudioSource = GetComponent<AudioSource>();

    }

    private int _positionIndex = 0;
    
    // アイテムの実行
    public void ExecItem(ItemType type)
    {
        if (type == ItemType.Empty)
        {
            _audioSource.PlayOneShot(noneitem , 0.2f); // アイテムがないときの使用時
        }
        else　if (type != ItemType.Flag)
        {
            _audioSource.PlayOneShot(useitem);//Flag以外のアイテム使用時音
        }
        // 種類ごとに処理を設定。
        // ここで制御できるようにしてあげることで、一括で速度アップの値の変更などができる
        switch (type)
        {
            case ItemType.Flag:
            {
                // 選択しているタイルの旗を切り替え
                foreach (var selectingTile in _tileSelectManager.SelectingTiles)
                {
                    selectingTile.ToggleFlag(flagPrefab);
                }
                break;
            }
            case ItemType.SpeedUp:
            {
                // 移動速度+2
                _playerController.AddMoveSpeedNumb(1f);
                break;
            }
            case ItemType.SpeedDown:
            {
                // 移動速度-2
                _playerController.AddMoveSpeedNumb(-0.3f);
                break;
            }
            case ItemType.changefootsteps:
            {
                _playerController.ChangeFootsteps(_changestepsAudioClip);
                //PlayerController.AudioSource =changefootsteps;
                break;
            }
            case ItemType.OpenFrontLine:
            {
                _tileSelectManager.OpenFrontLine();
                break;
            }
            case ItemType.RandomMovement:
                _playerController.RandomMovement();
                break;
            case ItemType.AddOpenPosition:
                if (OpenPositions.Length > _positionIndex)
                {
                    _tileSelectManager.AddOpenPosition(OpenPositions[_positionIndex]);
                    _positionIndex++;
                }
                break;
            case ItemType.ViewBombNumb:
                tilesManager.ViewBombNumbs();
                break;
        }
    }
}
