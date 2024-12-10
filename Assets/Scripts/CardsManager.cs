using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField]
    private CardRateAsset firstAsset;
    [SerializeField]
    private CardRateAsset cardRate;
    [SerializeField]
    private int length = 9; // 一片の長さ。length*lengthのマスが生成される
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private UIManager uiManager;

    private CardController[] _tileCards;
    private SquareMap _squareMap;

    // Start is called before the first frame update
    void Start()
    {
        // タイルカードの配列指定の長さで初期化
        _tileCards = new CardController[length * length];
        // 開始位置判定用のタイルカードを生成
        CreateMaps(firstAsset, Vector2Int.zero);
    }

    private void CreateMaps(CardRateAsset asset, Vector2Int startPos)
    {
        // タイルカードの配列を生成
        foreach (var tile in _tileCards)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        
        // アセットと開始位置、長さを指定してMapを生成
        _squareMap = new SquareMap(asset, startPos, length, length);
        uiManager.WriteMap(_squareMap);

        // マップのデータに沿ってタイルカードプレファブのインスタンスを生成
        for (int i = 0; i < _squareMap.Map.Length; i ++)
        {
            // カード情報を取得
            var cardInfo = asset.cardInfos[_squareMap.Map[i]];
            // タイルカードの座標を計算
            var cardPos = MapCalculation.GetCardPosition(i, length); // タイルカードの座標を取得
            var tileX = cardPos.x - _squareMap.Width / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
            var tileZ = cardPos.y - _squareMap.Height / 2;
            var tileVector = new Vector3(tileX, 0, tileZ);
            var tileQuaternion = Quaternion.identity; // 回転なし、0度の状態

            // タイルカードを生成
            var tileCard = Instantiate(cardInfo.cardPrefab, tileVector, tileQuaternion);
            // タイルカードを初期化
            tileCard.Initialize(i, cardInfo.cardType);
            // タイルカードをこのスクリプトがアタッチされているオブジェクトの子にする
            tileCard.transform.SetParent(transform);
            // タイルカードを配列に格納
            _tileCards[i] = tileCard;
            
            // 最初の位置選択の場合
            if (cardInfo.cardType == CardType.First)
            {
                // タイルカードが裏返されたときのイベントを購読し、初期位置選択時の処理を実行
                tileCard.OnFlipped.Subscribe(OnFirstFlipped);
            }
            else
            {
                // タイルカードが裏返されたときのイベントを購読し、(本番)タイルカードが裏返されたときの処理を実行
                tileCard.OnFlipped.Subscribe(OnCardFlipped);
            }
        }
    }

    // タイルカードが裏返されたときの処理
    private async void OnFirstFlipped(int cardId)
    {
        // 選択されたカードから初期位置を取得
        var cardPos = MapCalculation.GetCardPosition(cardId, length);
        // 本番タイルカードを生成
        CreateMaps(cardRate, cardPos);
        // カードインスタンスのStartメソッドが呼ばれるのを待つため、1フレーム待機(Startをなくしたほうがきれいではある)
        await UniTask.DelayFrame(1);
        // 初期位置(選択された)のカードを裏返す
        _tileCards[cardId].FlipCard();
    }
    
    // タイルカードが裏返されたときの処理
    private void OnCardFlipped(int cardId)
    {
        // カードのインスタンスをキャッシュ的な感じで取得
        var tileCard = _tileCards[cardId];
        // タイルカードが爆弾の場合はゲームオーバー
        if (tileCard.CardType == CardType.Bomb)
        {
            //Debug.Log("Game Over");
            // プレイヤーの位置とタイルカードの位置から方向を計算し、プレイヤーに衝撃を与える
            var playerPos = playerController.transform.position;
            var cardPos = tileCard.transform.position;
            var direction = (playerPos - cardPos).normalized;
            playerController.Impact(direction);
        }
        else
        {
            // タイルカードが爆弾でない場合は周囲の爆弾の数を取得
            var sum = GetCardAroundBombSum(cardId);

            // 周囲に爆弾がない場合
            if (sum == 0)
            {
                // 周囲のタイルカードを裏返す
                var aroundCards = MapCalculation.GetAroundCardIds(cardId, length, length * length); // 周辺取得何回か繰り返しちゃってるがまぁ気にしないこととする
                foreach (var aroundCardId in aroundCards)
                {
                    _tileCards[aroundCardId].FlipCard();
                }
            }
            else
            {
                // 周囲に爆弾がある場合は周囲の爆弾の数を表示
                tileCard.SetText(sum.ToString());
            }
        }
        // 残りのタイルがすべてボムかどうかをチェック
        Debug.Log("a");
    }
    
    // タイルカードの周囲の爆弾の数を取得
    private int GetCardAroundBombSum(int cardId)
    {
        var sum = 0;

        // 周囲のタイルカードのIDを取得
        var aroundCards = MapCalculation.GetAroundCardIds(cardId, length, length * length);
        // 周囲のタイルカードを調べる
        foreach (var aroundCardId in aroundCards)
        {
            //Debug.Log(aroundCardId);
            // タイルカードが爆弾の場合は加算
            if (_tileCards[aroundCardId].CardType == CardType.Bomb)
            {
                sum++;
            }
        }

        return sum;
    }
    private void CheckForOnlyBombs()
{
    bool onlyBombs = true;
    foreach (var tileCard in _tileCards)
    {
        if (tileCard.CardType != CardType.Bomb)
        {
            onlyBombs = false;
            break;
        }
    }

    if (onlyBombs)
    {
        Debug.Log("Clear! All cards are bombs.");
        // クリア処理をここに追加
    }
}
}
