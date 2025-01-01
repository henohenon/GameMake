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
    private InputActionReference mouse;//マウスの入力を取得
    [SerializeField]
    private InputActionReference move;//キャラクターの移動を取得
    [SerializeField]
    private float cameraRotationSpeed = 0.1f; // カメラの回転速度を調節するためのflot
    [SerializeField]
    private TilesManager tilesManager;
    
    private Rigidbody _rb;
    private Vector2 moveInput;
    private Vector2 cameraInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.position = new Vector3(0, 1f, 0);
    
        // 0の入力も受け取れるように、canceledも登録
        move.action.performed += MoveInput;
        move.action.canceled += MoveInput;
        move.action.Enable();
        
        mouse.action.performed += CameraInput;
        mouse.action.canceled += CameraInput;
        mouse.action.Enable();
    }
    
    private void CameraInput(InputAction.CallbackContext context)
    {
        // performed、canceledコールバックを受け取る
        if (context.started) return;
        
        cameraInput = context.ReadValue<Vector2>();
    }
    
    private void MoveInput(InputAction.CallbackContext context)
    {
        // performed、canceledコールバックを受け取る
        if (context.started) return;
        
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        // 移動方向を計算
        var moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        // 移動方向を正規化
        moveDirection.Normalize();
        // 移動方向をカメラの向きに合わせる
        moveDirection = Quaternion.Euler(0, _rb.rotation.eulerAngles.y, 0) * moveDirection;
        // 移動方向を適用
        _rb.position += moveDirection * (moveSpeed * Time.deltaTime);
        
        // カメラの回転座標の作成
        var addRotation = new Vector3(
            -cameraInput.y * cameraRotationSpeed,
            cameraInput.x * cameraRotationSpeed,
            0
        );
        var currentRotation = _rb.rotation.eulerAngles;
        var nextRotation = currentRotation + addRotation;
        // カメラの回転を適用
        _rb.MoveRotation(Quaternion.Euler(nextRotation));
    }

    public void Impact(Vector3 direction)
    {
        _rb.AddForce(direction * 10, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 10, ForceMode.Impulse);
    }
    
    private void OnDisable()
    {
        move.action.Disable();
        mouse.action.Disable();
    }
}

