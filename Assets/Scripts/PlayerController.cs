using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour//へのへのさん
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private InputActionReference space;
    [SerializeField]
    private AudioManager audioManager;

    private Rigidbody _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        space.action.performed += _ => audioManager.SetSE1(); //キャンセルとかもある
        space.action.Enable();
    }
    
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
        _rb.position += movement;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotation));        
        // スペースキーを押したら
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            // 自分の位置から下方向に2のRayを飛ばす
            if(Physics.Raycast(transform.position, -transform.up, out hit, 2))
            {
                // Rayが当たったオブジェクトを取得
                var hitObject = hit.collider.gameObject;
                // カードコントローラーがアタッチされていたら
                if (hitObject.TryGetComponent<CardController>(out var cardController))
                {
                    // カードを回転させる
                    cardController.FlipCard();
                }
            }
        }
    }
    
    public void Impact(Vector3 direction)
    {
        _rb.AddForce(direction * 10, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 10, ForceMode.Impulse);
    }
}
