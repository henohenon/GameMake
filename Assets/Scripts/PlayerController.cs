using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour//へのへのさん
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private InputActionReference space;//スペースキーでタイルを裏返す処理
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
        space.action.performed += _ => audioManager.PlayFlipTileEffect(); //キャンセルとかもある
        space.action.Enable();

        CameraAction.action.performed += RotCam; //キャンセルとかもある
        CameraAction.action.Enable();
    }

    private void Update()
    {
        // キー入力を取得 TODO: InputSystemに変更
        var horizontal = Input.GetAxis("Horizontal");//左右の入力
        var vertical = Input.GetAxis("Vertical");//前後の入力

        // 移動方向を計算
        var movement = (transform.forward * vertical) + (transform.right * horizontal); // 正面に対して前後の入力

        // スピードとフレームの経過時間をかける
        movement *= moveSpeed * Time.deltaTime;

        // 移動と回転
        _rb.position = new Vector3(_rb.position.x + movement.x, _rb.position.y, _rb.position.z + movement.z);

        // Frame update example: Draws a 10 meter long green line from the position for 1 frame.

            Vector3 down = Vector3.down * 2;//下方向にRayを飛ばす
        Debug.DrawRay(transform.position, down, Color.red);
        
        // スペースキーを押したら TODO: InputSystemに変更
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            RaycastHit hit;
            // 自分の位置から下方向に2のRayを飛ばす TODO: Raycastの方向を変更
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2)) // direction(向き)をに-↑変更最大２まで
            {
                // Rayが当たったオブジェクトを取得
                var hitObject = hit.collider.gameObject;
                // タイルコントローラーがアタッチされていたら
                if (hitObject.TryGetComponent<TileController>(out var tileController))
                {
                    // タイルを回転させる
                    tileController.Open();
                }
            }
        }
    }

    public void Impact(Vector3 direction)
    {
        audioManager.PlayExplosionEffect();
        _rb.AddForce(direction * 10, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 10, ForceMode.Impulse);
    }
    void RotCam(InputAction.CallbackContext context)
    {
        // performed、canceledコールバックを受け取る
        if (context.started) return;

        if (Mouse.current.leftButton.isPressed)
        {
            // Moveアクションの入力取得
            var inputMove = context.ReadValue<Vector2>();
            // カメラの回転座標の作成
            var addRotation = new Vector3( // 2d空間と3d空間で軸が違うので注意
                -inputMove.y * cameraRotationSpeed, // -逆だったりしたらごめんなさい
                inputMove.x * cameraRotationSpeed,
                0
            );
            var currentRotation = _rb.rotation.eulerAngles;
            var nextRotation = currentRotation + addRotation;
            // カメラの回転を適用
            _rb.MoveRotation(Quaternion.Euler(nextRotation));
        }
    }
}

