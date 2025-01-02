using System;
using Alchemy.Inspector;
using UnityEngine;

namespace Scriptable
{
    // クラスはserializableをつけるとインスペクターでいじれるようになる(シリアル化可能な変数で構成されている場合に限る)
    [CreateAssetMenu(fileName = "MapRateData", menuName = "MapRateData")]
    public class MapRateAsset: ScriptableObject
    {
        [AssetsOnly]
        public GameObject[] randomTiles;
        public EachTileInfo[] tileRateInfos;
    }
    
    [Serializable]
    public class EachTileInfo: RateItemBase
    {
        public TileType tileType;
    }

    public enum TileType
    {
        Safety,
        Bomb,
        BlueItem,
        YellowItem,
        PurpleItem,
    }

}

