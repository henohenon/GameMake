using System;
using Alchemy.Inspector;
using UnityEngine;

namespace Scriptable
{
    [Serializable] // クラスはserializableをつけるとインスペクターでいじれるようになる(シリアル化可能な変数で構成されている場合に限る)
    public class EachTileInfo: RateItemBase
    {
        [AssetsOnly]
        public TileController tilePrefab;
        public TileType tileType;
    }

}
public enum TileType
{
    Safety,
    Bomb,
    BlueItem,
    YellowItem,
    PurpleItem,
}
