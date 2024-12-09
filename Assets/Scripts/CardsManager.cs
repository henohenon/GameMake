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
                var cardId = x * length + z;
                // ランダムなカードを取得
                var randomCard = cardRate.GetRandomCard();
                
                // タイルカードを生成
                var tileCard = Instantiate(randomCard, tileVector, tileQuaternion);
                // カードのIDを設定
                tileCard.Initialize(cardId);
                // タイルカードをこのスクリプトがアタッチされているオブジェクトの子にする
                tileCard.transform.SetParent(transform);
                // タイルカードを配列に格納
                _tileCards[cardId] = tileCard;

                // タイルカードが裏返されたときのイベントを購読
                tileCard.OnFlipped.Subscribe(cardId =>
                {
                    Debug.Log($"Card {cardId} is flipped!");
                });
            }
        }
    }
}
