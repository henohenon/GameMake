using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Camera))]
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
    private Camera _camera;
    private float _nowMoveSpeed;
    private Vector2 _moveInputValue;
    private Vector2 _cameraInputValue;
    private Subject<Unit> _onDamage;
    public Observable<Unit> OnDamage => _onDamage;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        _rb = GetComponent<Rigidbody>();
        _rb.position = new Vector3(0, 1f, 0);
    
        // 各種入力の登録
        moveInput.action.performed += MoveInputCallback;
        moveInput.action.canceled += MoveInputCallback; // 0の入力も受け取れるように、canceledも登録
        moveInput.action.Enable(); // enableしないと入力を受け取れない
        
        cameraInput.action.performed += CameraInputCallback;
        cameraInput.action.canceled += CameraInputCallback;
        // cameraInput.action.Enable(); カーソルがロックされるまで入力を受け取らない
        
        cameraLock.action.started += CameraLockCallback;
        cameraLock.action.Enable();
        
        // 移動速度の計算
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
        if (_isMovementPose) return;
        
        // 入力に併せてカメラを固定
        var input = context.ReadValue<float>();
        SetCameraLock(input == 1);
    }

    private bool _isMovementPose = false;
    public void MovementPose()
    {
        _isMovementPose = true;
        _rb.freezeRotation = false;
        _camera.nearClipPlane = 0.01f;
    }

    public void SetCameraLock(bool isLock)
    {
        if(isLock){
            cameraInput.action.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            cameraInput.action.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void Update()
    {
        if(_isMovementPose) return;
        // 移動入力
        var moveDirection = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
        // y軸の向きと入力を掛け合わせ、今向いてる方向に平行移動
        var movement = Quaternion.Euler(0, _rb.rotation.eulerAngles.y, 0) * moveDirection;
        // 移動。現座標 + 移動の向き * 速度 * 経過時間
        _rb.position += movement * (_nowMoveSpeed * Time.deltaTime);
        
        // 入力
        var addRotation = new Vector3(
            -_cameraInputValue.y,
            _cameraInputValue.x,
            0
        );
        // 今の角度
        var currentRotation = _rb.rotation.eulerAngles;
        // 次の角度。今の角度 + 入力 * スピード
        var nextRotation = currentRotation + addRotation * cameraRotationSpeed;
        // カメラの回転を適用
        _rb.MoveRotation(Quaternion.Euler(nextRotation));
    }

    // 爆発で吹っ飛ぶ
    public void Impact(Vector3 direction)
    {
        _rb.AddForce(direction * 1f, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 1f, ForceMode.Impulse);
    }

    private readonly List<float> _addMoveSpeeds = new();
    public void AddMoveSpeedNumb(float speed)
    {
        // 速度を追加
        _addMoveSpeeds.Add(speed);
        // 再計算
        CalcMoveSpeed();
    }

    // 最小の移動速度
    private const float MinMoveSpeed = 0.05f;
    // 移動速度の再計算関数
    private void CalcMoveSpeed()
    {
        // デフォルト値
        _nowMoveSpeed = defaultMoveSpeed;

        // 外部から追加された速度をすべて加算
        foreach (var addSpeed in _addMoveSpeeds)
        {
            _nowMoveSpeed += addSpeed;
        }

        // もし最小値よりも小さい場合は最小値にする
        if (_nowMoveSpeed < MinMoveSpeed)
        {
            _nowMoveSpeed = MinMoveSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Damage"))
        {
            _onDamage.OnNext(Unit.Default);
        }
    }

    // inputactionはdisableしとかないと怒られる
    private void OnDisable()
    {
        moveInput.action.Disable();
        cameraInput.action.Disable();
        cameraLock.action.Disable();
    }
    
    public enum PlayerPoseType
    {
        FpsLock,
        FpsNotLock,
        UIPose,
    }
}

