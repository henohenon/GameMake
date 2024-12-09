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
                tileCard.OnFlipped.Subscribe(cardId =>
                {
                    // タイルカードが爆弾の場合はゲームオーバー
                    if (_tileCards[cardId].CardType == CardType.Bomb)
                    {
                        Debug.Log("Game Over");
                    }
                    else
                    {
                        // タイルカードが爆弾でない場合は周囲の爆弾の数を取得
                        var sum = GetCardAroundBombSum(cardId);
                        Debug.Log(sum);
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
            var aroundCardId = GetCardId(aroundPosition);
            // タイルカードが存在しない場合はスキップ
            if (aroundCardId < 0 || aroundCardId >= _tileCards.Length) continue;
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
