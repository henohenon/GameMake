using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTileCards : MonoBehaviour
{
    [SerializeField]
    private CardRateAsset cardRate;
    [SerializeField]
    private int length = 9;
    
    // Start is called before the first frame update
    void Start()
    {
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
                
                var randomCard = cardRate.GetRandomCard(); // ランダムなカードを取得
                
                // タイルカードを生成
                var tileCard = Instantiate(randomCard, tileVector, tileQuaternion); 
                // タイルカードをこのスクリプトがアタッチされているオブジェクトの子にする
                tileCard.transform.SetParent(transform);
            }
        }
    }
}
