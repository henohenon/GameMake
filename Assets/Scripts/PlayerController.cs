using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    
    private void Update()
    {
        // キー入力を取得
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        
        // 移動方向を計算
        var movement = transform.forward * vertical; // 正面に対して前後の入力
        
        // 回転方向を計算
        var rotation = Vector3.up * horizontal; // 上方向に対して左右の入力
        
        // スピードとフレームの経過時間をかける
        movement *= moveSpeed * Time.deltaTime;
        rotation *= rotateSpeed * Time.deltaTime;
        
        // 移動と回転
        transform.position += movement;
        transform.Rotate(rotation);
        
        // スペースキーを押したら
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            // 自分の位置から下方向に2のRayを飛ばす
            if(Physics.Raycast(transform.position, -transform.up, out hit, 2))
            {
                // 当たったやつをxxする
                var hitObject = hit.collider.gameObject;
                Destroy(hitObject);
            }
        }
    }
}
