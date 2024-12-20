```mermaid
classDiagram

class TilesManager
class TileController
namespace pureCSharpクラス{
    class MapCalculator{
        +GetPosId(Vector2Int pos):int$
        +GetIdPos(int id):Vector2Int$
        +GetIdAroundIds(id pos):Vector2[]$
    }
    class TileRateInfo{
        +TileController tilePrefab
        +TileType tileType
        +int rate
    }
    class ItemInfo{
    }
    class MapInfo{
        +readonly int[] map
        +readonly int width
        +readonly int height
    }
    class GameInfo{
        +readonly ItemInfo itemInfo
        +readonly MapInfo mapInfo
        +readonly string shareId
    }
}
TileRateInfo *-- TilePrefab: プレファブの参照を持つ
TileRateInfo --o TileRateAsset: 複数のタイル情報を持つ
TileRateInfo *-- TileType


MapInfo --o RandomGameValues
ItemInfo --o RandomGameValues
TileRateAsset --* RandomGameValues: レートアセット情報を持つ


namespace scriptableObjects{
    class TileRateAsset{
        +TileRateInfo[] assets
        +GetRandomTile()
    }
    class RandomGameValues{
        +int Width
        +int Height
        +List<MapInfo[]> RandomGameValues
        +List<ItemInfo[]> randomItems
        +GetRandomData(): GameInfo
        -GenerateRandoms(int numbs)
    }
}
namespace enum{
    class TileType{
        Normal
        Bomb
        Item
    }
}

class ArmyGameManager
class ArmySoundManager
class ArmyPlayerController{
    +Impact(Vector3 direction)
    +PoseInput(bool isPose)
}
class ArmyItemStackManager
class ArmyGameUIManager{
    +ToGameOver()
    +ToGameClear()
    +GameStart(float time)
    +SetShareId(string shareId)
ArmyGameManager --> ArmyItemStackManager: アイテム追加/削除
ArmyUIDocument <-- ArmyItemStackManager
}

GameInfo *-- TilesManager: ランダムゲーム情報取得の戻り値
RandomGameValues <-- TilesManager: ランダムゲーム情報の取得
GameInfo *-- RandomGameValues: ゲーム情報取得の戻り値

TilesManager --> ArmyGameUIManager: 共有IDの表示
TilesManager --> TileController: プレファブの生成

TileController <|-- BombTileController: 継承

BombPrefab <-- BombTileController

ArmyPlayerController --> TileController: 効果の発動
ArmyPlayerController --> TileController: フォーカス

ArmyGameManager --> ArmySoundManager
ArmyGameManager --> ArmyGameUIManager: ゲームオーバー/ゲームクリア/ゲームスタート

ArmyGameManager --> ArmyPlayerController: ポーズの設定

PlayerObject <-- ArmyPlayerController: 移動/カメラ制御/吹っ飛びなど

TilePrefab <-- TileController
ArmyUIDocument <-- ArmyGameUIManager

TilePrefab <|-- BombPrefab: prefab variant
namespace Unity空間 {
    class ArmyUIDocument
    class MainUIDocument
    class CommanderUIDocument
    class TilePrefab
    class BombPrefab
    class PlayerObject
}
CommanderUIManager --> CommanderUIDocument
CommanderGameManager --> CommanderUIManager
namespace CommanderUI{
    class CommanderUIManager{
        +CreateMap(MapInfo info, TileRateAsset asset)
        +CreateItem(ItemInfo info)
    }
    class CommanderGameManager{
        +GenerateUI(string shareId)
    }
}

MainUIDocument <-- MainGameManager
class MainGameManager{
    +ToMainMenu()
}

```

Army=操作プレイヤー
Commander=操作プレイヤー

MapCalculatorはstatic関数。周りの爆弾の数字計算したりするときに使う→アイテム次第では不要かも
RandomMapsはTileRateAssetで定義した割合情報とそれをもとに生成したマップとアイテムの乱数を持つ
- 非実行時に実行可能なマップとアイテムのランダム生成関数を持つ
- ランダムなデータとその共有IDの取得

ItemStackManagerはUI管理も兼任する