using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField]
    private CardRateAsset cardRate;
    [SerializeField]
    private int length = 9;
    [SerializeField]
    private PlayerController playerController;
    
    private CardController[] _tileCards;
    private SquareMap _squareMap;
    
    // Start is called before the first frame update
    void Start()
    {
        // タイルカードの配列を生成
        _tileCards = new CardController[length * length];

        var startPos = Vector2Int.zero;
        _squareMap = new SquareMap(cardRate, startPos, length, length);

        for (int i = 0; i < _squareMap.Map.Length; i ++)
        {
            // カード情報を取得
            var cardInfo = cardRate.cardInfos[_squareMap.Map[i]];
            // タイルカードの座標を計算
            var cardPos = _squareMap.GetCardPosition(i); // タイルカードの座標を取得
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

            
            // タイルカードが裏返されたときのイベントを購読
            tileCard.OnFlipped.Subscribe(OnCardFlipped);
        }
    }

    private void OnCardFlipped(int cardId)
    {
        var tileCard = _tileCards[cardId];
        // タイルカードが爆弾の場合はゲームオーバー
        if (tileCard.CardType == CardType.Bomb)
        {
            Debug.Log("Game Over");
            var playerPos = playerController.transform.position;
            var cardPos = tileCard.transform.position;
            var direction = (playerPos - cardPos).normalized;
            playerController.Impact(direction);
        }
        else
        {
            // タイルカードが爆弾でない場合は周囲の爆弾の数を取得
            var sum = GetCardAroundBombSum(cardId);

            // 周囲に爆弾がない場合は周囲のタイルカードを裏返す
            if (sum == 0)
            {
                var aroundCards = _squareMap.GetAroundCardIds(cardId); // 周辺取得何回か繰り返しちゃってるがまぁ気にしないこととする
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
    }
    
    
    private int GetCardAroundBombSum(int cardId)
    {
        var sum = 0;
        
        // 周囲のタイルカードのIDを取得
        var aroundCards = _squareMap.GetAroundCardIds(cardId);
        // 周囲のタイルカードを調べる
        foreach (var aroundCardId in aroundCards)
        {
            // タイルカードが爆弾の場合は加算
            if (_tileCards[aroundCardId].CardType == CardType.Bomb)
            {
                sum++;
            }
        }

        return sum;
    }
}
