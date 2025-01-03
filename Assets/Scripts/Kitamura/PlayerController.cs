using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour//へのへのさん
{
    [SerializeField]
    private float defaultMoveSpeed;
    [SerializeField]
    private float defaultRotateSpeed;
    [SerializeField]
    private InputActionReference cameraInput;//マウスの入力を取得
    [SerializeField]
    private InputActionReference moveInput;//キャラクターの移動を取得
    [SerializeField] private InputActionReference cameraLock;
    [SerializeField]
    private float cameraRotationSpeed = 0.1f; // カメラの回転速度を調節するためのflot
    [SerializeField]
    private TilesManager tilesManager;
    
    private Rigidbody _rb;
    private float _nowMoveSpeed;
    private Vector2 _moveInputValue;
    private Vector2 _cameraInputValue;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.position = new Vector3(0, 1f, 0);
    
        // 0の入力も受け取れるように、canceledも登録
        moveInput.action.performed += MoveInputCallback;
        moveInput.action.canceled += MoveInputCallback;
        moveInput.action.Enable();
        
        cameraInput.action.performed += CameraInputCallback;
        cameraInput.action.canceled += CameraInputCallback;
        
        cameraLock.action.started += CameraLockCallback;
        cameraLock.action.Enable();
        
        CalcMoveSpeed();
    }
    
    private void CameraInputCallback(InputAction.CallbackContext context)
    {
        // performed、canceledコールバックを受け取る
        if (context.started) return;
        
        _cameraInputValue = context.ReadValue<Vector2>();
    }
    
    private void MoveInputCallback(InputAction.CallbackContext context)
    {
        // performed、canceledコールバックを受け取る
        if (context.started) return;
        
        _moveInputValue = context.ReadValue<Vector2>();
    }
    
    private void CameraLockCallback(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        var input = context.ReadValue<float>();
        if (input == 1)
        {
            cameraInput.action.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (input == -1)
        {
            cameraInput.action.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void Update()
    {
        // 移動方向を計算
        var moveDirection = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
        // 移動方向を正規化
        moveDirection.Normalize();
        // 移動方向をカメラの向きに合わせる
        moveDirection = Quaternion.Euler(0, _rb.rotation.eulerAngles.y, 0) * moveDirection;
        // 移動方向を適用
        _rb.position += moveDirection * (_nowMoveSpeed * Time.deltaTime);
        
        // カメラの回転座標の作成
        var addRotation = new Vector3(
            -_cameraInputValue.y * cameraRotationSpeed,
            _cameraInputValue.x * cameraRotationSpeed,
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

    private List<float> addMoveSpeeds = new();
    public void AddMoveSpeedNumb(float speed)
    {
        addMoveSpeeds.Add(speed);
        CalcMoveSpeed();
    }

    private float minMoveSpeed = 0.05f;
    private void CalcMoveSpeed()
    {
        _nowMoveSpeed = defaultMoveSpeed;

        foreach (var addSpeed in addMoveSpeeds)
        {
            _nowMoveSpeed += addSpeed;
        }

        if (_nowMoveSpeed < minMoveSpeed)
        {
            _nowMoveSpeed = minMoveSpeed;
        }
    }
    
    private void OnDisable()
    {
        moveInput.action.Disable();
        cameraInput.action.Disable();
        cameraLock.action.Disable();
    }
}

