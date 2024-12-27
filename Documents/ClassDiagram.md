```mermaid
classDiagram
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
    class GameInfo{
        +readonly int randomItemId
        +readonly int randomMapId
        +readonly string shareId
    }
}
class TileController{
    +FlipTile()
}
GameInfo *-- RandomGameValues: ゲーム情報取得の戻り値

TileRateInfo *-- TilePrefab: プレファブの参照を持つ
TileRateInfo --o TileRateAsset: 複数のタイル情報を持つ
TileRateInfo *-- TileType

TileRateAsset --* RandomGameValues: レートアセット情報を持つ
ItemRateAsset --* RandomGameValues: アイテムレートアセット情報を持つ


ArmyGameManager --> RandomGameValues : ランダムなゲーム情報取得
CommanderGameManager --> RandomGameValues : 共有IDからゲーム情報取得
namespace プレイヤー処理 {
    class ArmyGameManager
    class CommanderGameManager
}
namespace scriptableObjects{
    class TileRateAsset{
        +TileRateInfo[] assets
        +GetRandomTile()
    }
    class ItemRateAsset{
        +ItemRateInfo[] assets
        +GetRandomItem()
    }
    class RandomGameValues{
        +int Width
        +int Height
        +TileRateAsset tileRateAsset
        +List<int[]> randomMaps
        +ItemRateAsset itemRateAsset
        +List<int[]> randomItems
        +GetRandomData(): GameInfo
        +GetShareIdData(string shareId): GameInfo
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


TileController <|-- ItemTileController: 継承
TileController <|-- BombTileController: 継承

BombPrefab <-- BombTileController
ItemPrefab <-- ItemTileController


TilePrefab <-- TileController

TilePrefab <|-- BombPrefab: prefab variant
TilePrefab <|-- ItemPrefab: prefab variant

namespace Unity空間 {
    class TilePrefab
    class BombPrefab
    class ItemPrefab
}


```
MapCalculatorはstatic関数。周りの爆弾の数字計算したりするときに使う→アイテム次第では不要かも
RandomMapsはTileRateAsset/ItemRateAssetで定義した割合情報とそれをもとにランダムなマップ/アイテムセットを生成、保持する(listのrandomMaps/randomItems)
- 非実行時に実行可能なマップとアイテムのランダム生成関数を持つ

```mermaid
classDiagram
    ArmyUIDocument <-- ArmyGameUIManager
    TileController　--> TilePrefab
    TilesManager --> TileController: タイルの生成
    TileController <-- TilePrefab: タイルがめくられたコールバック
    TilesManager --> ArmyGameManager: ゲームクリアコールバック
    ArmyGameManager --> ArmyGameUIManager: ゲームオーバー/ゲームクリア/ゲームスタート/共有ID
    ArmyPlayerController <-- ArmyGameManager: 移動/カメラ制御/吹っ飛び/ポーズ設定など
    ArmyGameManager --> TilesManager: タイルの生成
    ArmyGameUIManager --> ArmyGameManager: タイマー0のコールバックイベント
    ArmyGameManager --> ArmySoundManager: 効果音再生
    ArmyGameManager --> RandomGameValues: ランダムなゲームデータの取得
    ArmyPlayerController　--> PlayerObject
    
    class ArmyGameManager{
        +GameStart(float time)
        +SetShareId(string shareId)
    }
    class ArmySoundManager
    class ArmyItemStackManager
    class ArmyGameUIManager{
        +ToGameOver()
        +ToGameClear()
        +GameStart(float time)
        +SetShareId(string shareId)
    }
    class ArmyPlayerController{
        +Impact(Vector3 direction)
        +PoseInput(bool isPose)
    }
    ArmyGameManager --> ArmyItemStackManager: アイテム追加/削除
    ArmyUIDocument <-- ArmyItemStackManager
    ArmyGameManager <-- MainGameManager: ゲームスタート
    ArmyGameManager --> MainGameManager: ゲームオーバー/ゲームクリア
    
    namespace Unity空間 {
        class TilePrefab
        class ArmyUIDocument
        class PlayerObject
    }
    namespace 外部{
        class RandomGameValues
        class MainGameManager
     }
```

```mermaid
classDiagram
RandomGameValues <-- CommanderGameManager: 共有IDからゲームデータ取得
CommanderUIManager --> CommanderUIDocument
CommanderGameManager --> CommanderUIManager
CommanderGameManager <-- CommanderUIManager: ボタンなどが押されたときのコールバック
class CommanderUIManager{
    +CreateMap(MapInfo info, TileRateAsset asset)
    +CreateItem(ItemInfo info)
}
class CommanderGameManager{
    +GenerateUI(string shareId)
    +StartGame()
}

CommanderGameManager <-- MainGameManager: ゲームスタート
CommanderGameManager --> MainGameManager: ゲーム終了

namespace 外部 {
    class MainGameManager
    class RandomGameValues
}
```