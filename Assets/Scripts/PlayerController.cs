using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField]
    private InputActionReference CameraAction;
    [SerializeField]
    private float cameraRotationSpeed = 0.1f; // カメラの回転速度を調節するためのflot



    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.position = new Vector3(0, 1f, 0);
        space.action.performed += _ => audioManager.SetSE1(); //キャンセルとかもある
        space.action.Enable();

        CameraAction.action.performed += RotCam; //キャンセルとかもある
        CameraAction.action.Enable();
    }

    private void Update()
    {
        // キー入力を取得
        var horizontal = Input.GetAxis("Horizontal");//左右の入力
        var vertical = Input.GetAxis("Vertical");//前後の入力

        // 移動方向を計算
        //var movement = transform.forward * vertical; // 正面に対して前後の入力
        var movement = (transform.forward * vertical) + (transform.right * horizontal); // 正面に対して前後の入力
        //var movement = new Vector3(horizontal, 0, vertical); // 正面に対して前後の入力 aさんのやつ


        // 回転方向を計算
        //var rotation = Vector3.up * horizontal; // 上方向に対して左右の入力

        // スピードとフレームの経過時間をかける
        movement *= moveSpeed * Time.deltaTime;
        //rotation *= rotateSpeed * Time.deltaTime;

        // 移動と回転
        //_rb.position += movement;
        _rb.position = new Vector3(_rb.position.x + movement.x, _rb.position.y, _rb.position.z + movement.z);


        // スペースキーを押したら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            // 自分の位置から下方向に2のRayを飛ばす
            if (Physics.Raycast(transform.position, -transform.up, out hit, 2))
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
        var rotation = _rb.rotation.eulerAngles;
        rotation.z = 0;
        _rb.rotation = Quaternion.Euler(rotation);
    }

    public void Impact(Vector3 direction)
    {
        //ここに爆発のSEを入れたい
        audioManager.SetSE2();
        Debug.Log("Impact");
        //ここに爆発のSEを入れたい
        _rb.AddForce(direction * 10, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 10, ForceMode.Impulse);
    }
    void RotCam(InputAction.CallbackContext context)
    {
        
        // performed、canceledコールバックを受け取る
        if (context.started) return;
        //Debug.Log("rotcam1");
        if (Mouse.current.leftButton.isPressed)
        {
            // Moveアクションの入力取得
            var inputMove = context.ReadValue<Vector2>();
            var swappedInputMove = new Vector2(-inputMove.y, inputMove.x);
            //Debug.Log(inputMove);
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(swappedInputMove.x * cameraRotationSpeed, swappedInputMove.y * cameraRotationSpeed, 0));
        }
    }
}

