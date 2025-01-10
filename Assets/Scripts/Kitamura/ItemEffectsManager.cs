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
    ViewAroundBombNumb = 8,
}

public class ItemEffectsManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TileSelectManager _tileSelectManager;
    [SerializeField, AssetsOnly] private GameObject flagPrefab;
    [SerializeField] private Light light;

    private LightDistanceType _currentLightDistanceType = LightDistanceType.Normal;

    private void Start()
    {
        _lightDistances[_currentLightDistanceType].ApplyValues(light);
    }

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
            case ItemType.LightUp://霧の視界綺麗に
                {
                    switch (_currentLightDistanceType)
                    {
                        case LightDistanceType.Normal:
                            _currentLightDistanceType = LightDistanceType.Maximum;
                            break;
                        case LightDistanceType.Minimum:
                            _currentLightDistanceType = LightDistanceType.Normal;
                            break;
                    }

                    _lightDistances[_currentLightDistanceType].ApplyValues(light);
                    break;
                }
            case ItemType.LightDown://霧の視界綺麗に
                {
                    switch (_currentLightDistanceType)
                    {
                        case LightDistanceType.Normal:
                            _currentLightDistanceType = LightDistanceType.Minimum;
                            break;
                        case LightDistanceType.Maximum:
                            _currentLightDistanceType = LightDistanceType.Normal;
                            break;
                    }

                    _lightDistances[_currentLightDistanceType].ApplyValues(light);
                    break;
                }
        }
    }
    
    private readonly Dictionary<LightDistanceType, LightDistanceValues> _lightDistances = new ()
    {
        {LightDistanceType.Normal, new LightDistanceValues(0.3f, 0.2f, 7.5f)},
        {LightDistanceType.Minimum, new LightDistanceValues(0.1f, 0.075f, 2f)},
        {LightDistanceType.Maximum, new LightDistanceValues(0.45f, 0.3f, 10f)},
    };
    
    private class LightDistanceValues
    {
        public float ambientIntensity;
        public float reflectionIntensity;
        public float lightRange;

        public LightDistanceValues(float ambientIntensity, float reflectionIntensity, float lightRange)
        {
            this.ambientIntensity = ambientIntensity;
            this.reflectionIntensity = reflectionIntensity;
            this.lightRange = lightRange;
        }

        public void ApplyValues(Light light)
        {
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            light.range = lightRange;
        }
    }
    
    private enum LightDistanceType
    {
        Normal = 0,
        Minimum = 1,
        Maximum = 2,
    }
}
