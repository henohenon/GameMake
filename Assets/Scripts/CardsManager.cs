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
    
    // Start is called before the first frame update
    void Start()
    {
        // タイルカードの配列を生成
        _tileCards = new CardController[length * length];
        
        // xをlength回繰り返す
        for(int x = 0; x < length; x++)
        {
            // zをlength回繰り返す
            for (int z = 0; z < length; z++)
            {
                // タイルカードの座標を計算
                var tileX = x - length / 2; // 真ん中のタイルが真ん中になるようにタイルの半分を引く
                var tileZ = z - length / 2;
                var tileVector = new Vector3(tileX, 0, tileZ);
                var tileQuaternion = Quaternion.identity; // 回転なし、0度の状態
                // カードのIDを計算
                var cardId = GetCardId(new Vector2Int(x, z));
                // ランダムなカードを取得
                var randomCard = cardRate.GetRandomCard();
                
                // タイルカードを生成
                var tileCard = Instantiate(randomCard.cardPrefab, tileVector, tileQuaternion);
                // タイルカードを初期化
                tileCard.Initialize(cardId, randomCard.cardType);
                // タイルカードをこのスクリプトがアタッチされているオブジェクトの子にする
                tileCard.transform.SetParent(transform);
                // タイルカードを配列に格納
                _tileCards[cardId] = tileCard;

                // タイルカードが裏返されたときのイベントを購読
                tileCard.OnFlipped.Subscribe(_ =>
                {
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
                            var aroundCards = GetAroundCardIds(cardId); // 周辺取得何回か繰り返しちゃってるがまぁ気にしないこととする
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
                });
            }
        }
    }
    
    private Vector2Int GetCardPosition(int cardId)
    {
        var x = cardId / length;
        var z = cardId % length;
        return new Vector2Int(x, z);
    }
    
    private int GetCardId(Vector2Int position)
    {
        return position.x * length + position.y;
    }

    private List<int> GetAroundCardIds(int cardId)
    {
        // 指定のタイルカードの座標を取得
        var position = GetCardPosition(cardId);
        // 指定のタイルカードの周囲の座標の一覧を作成
        var aroundPositions = new []
        {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.right,
            position + Vector2Int.left,
            position + Vector2Int.up + Vector2Int.right,
            position + Vector2Int.up + Vector2Int.left,
            position + Vector2Int.down + Vector2Int.right,
            position + Vector2Int.down + Vector2Int.left
        };
        // 周囲のタイルカードのIDを取得
        var aroundCardIds = new List<int>();
        foreach (var aroundPosition in aroundPositions)
        {
            // タイルカードが存在しない場合はスキップ
            if (aroundPosition.x < 0 || aroundPosition.x >= length || aroundPosition.y < 0 || aroundPosition.y >= length)
            {
                continue;
            }
            // タイルカードのIDを取得
            var aroundCardId = GetCardId(aroundPosition);
            // 周囲のタイルカードのIDを追加
            aroundCardIds.Add(aroundCardId);
        }
        return aroundCardIds;
    }
    
    private int GetCardAroundBombSum(int cardId)
    {
        var sum = 0;
        
        // 周囲のタイルカードのIDを取得
        var aroundCards = GetAroundCardIds(cardId);
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
