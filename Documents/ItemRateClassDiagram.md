# アイテムの割合について
## ScriptableObjectについて
データを保存できるやつ。データの型を定義→アセットを作成で特定の型のデータを無数に量産できる。
して難易度に合わせて複数のデータを作成、スクリプトにアタッチするものを変えるだけで難易度を変えるということができる
**定義**
- GameRateAsset
- MapRateAsset
- ItemRateAsset
**データ**
- GameRateData
- MapRateData
- ItemRateData

データのほうは名前は自由につけれる。

## Asset構造
```marmeid
classDiagram
GameRateAsset --> ItemRateData: まとめて持つ
GameRateAsset --> MapRateData: まとめて持つ
```