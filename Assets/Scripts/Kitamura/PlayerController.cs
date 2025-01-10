using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using RandomExtensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private Camera camera;
    [SerializeField]
    private TilesManager tilesManager;
    [SerializeField] private TileSelectManager tileSelectManager;
    [SerializeField] private Volume volume;
    
    private DepthOfField _dof;
    private LensDistortion _distortion;
    private Vignette _vignette;
    
    private Rigidbody _rb;
    private AudioSource _stepsAudioSource;
    private float _nowMoveSpeed;
    private Vector2 _moveInputValue;
    private Vector2 _cameraInputValue;
    private readonly Subject<Unit> _onDamage = new();
    public Observable<Unit> OnDamage => _onDamage;

    private void Start()
    {
        _stepsAudioSource = GetComponent<AudioSource>();
        _stepsAudioSource.Pause();
        
        _rb = GetComponent<Rigidbody>();
        _rb.position = new Vector3(0, 1f, 0);

        volume.profile.TryGet(out _dof);
        volume.profile.TryGet(out _distortion);
        volume.profile.TryGet(out _vignette);
    
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
        Debug.Log(_moveInputValue);
        if (_moveInputValue == Vector2.zero)
        {
            _stepsAudioSource.Pause();
        }
        else
        {
            _stepsAudioSource.UnPause();
        }
    }
    
    private void CameraLockCallback(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (_isDeadPose) return;
        
        // 入力に併せてカメラを固定
        var input = context.ReadValue<float>();
        SetCameraLock(input == 1);
    }

    private bool _isDeadPose = false;
    public void DeadPose()
    {
        // 各種アクションの無効化
        moveInput.action.Disable();
        cameraInput.action.Disable();
        cameraLock.action.Disable();
        
        // フラグ
        _isDeadPose = true;

        // 自由回転
        _rb.freezeRotation = false;
        // 地面に近づいても見えるように
        camera.nearClipPlane = 0.01f;
        // 足音を消す
        _stepsAudioSource.Pause();
        
        // 吹っ飛ぶ
        _rb.AddForce(_hitDirection * 300, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(_hitDirection, Vector3.up); // 適当にgptに吐かせた。なにやってるのかわかってない
        _rb.AddTorque(torqueAxis * 300f, ForceMode.Impulse);
        
        var animTime = 10f;
        
        // カメラ揺れ
        LMotion.Shake.Create(0, 1.5f, 1.3f).WithFrequency(3).WithEase(Ease.OutExpo).BindToLocalPositionX(camera.transform);
        LMotion.Shake.Create(0, 1.5f, 1.3f).WithFrequency(3).WithEase(Ease.OutExpo).BindToLocalPositionY(camera.transform);
        
        // 周り黒
        LMotion.Create(_vignette.smoothness.value, 1f, animTime*2).WithEase(Ease.OutExpo).Bind(_vignette.smoothness, (x, target) =>
        {
            target.value = x;
        });
        LMotion.Create(_vignette.intensity.value, 0.4f, animTime*2).WithEase(Ease.OutExpo).Bind(_vignette.intensity, (x, target) =>
        {
            target.value = x;
        });
        // ぼかす
        LMotion.Create(_dof.focusDistance.value, 300f, animTime).WithEase(Ease.OutExpo).Bind(_dof.focusDistance, (x, target) =>
        {
            target.value = x;
        });
        LMotion.Create(_dof.focalLength.value, 90f, animTime).WithEase(Ease.OutExpo).Bind(_dof.focalLength, (x, target) =>
        {
            target.value = x;
        });
        
        // 魚眼
        LMotion.Create(_distortion.intensity.value, 0.6f, animTime).WithEase(Ease.OutExpo).Bind(_distortion.intensity, (x, target) =>
        {
            target.value = x;
        });
        // FOV
        LMotion.Create(camera.fieldOfView, 35f, animTime).WithEase(Ease.OutExpo).Bind(camera, (x, target) =>
        {
            target.fieldOfView = x;
        });
    }

    public void ClearPose()
    {
        cameraInput.action.Disable();
        cameraLock.action.Disable();
    }

    public void SetCameraLock(bool isLock)
    {
        if(isLock){
            cameraInput.action.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            tileSelectManager.SelectPose(false);
        }
        else
        {
            cameraInput.action.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            tileSelectManager.SelectPose(true);
        }
    }
    
    private void Update()
    {
        if(_isDeadPose) return;
        // 移動入力
        var moveDirection = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
        // y軸の向きと入力を掛け合わせ、今向いてる方向に平行移動
        var movement = Quaternion.Euler(0, _rb.rotation.eulerAngles.y, 0) * moveDirection;
        // 移動。現座標 + 移動の向き * 速度 * 経過時間
        _rb.position += movement * (_nowMoveSpeed * Time.deltaTime);
        
        // カメラ横軸
        // 今の角度
        var currentRotation = _rb.rotation.eulerAngles;
        // x軸回転を加算
        currentRotation.y += _cameraInputValue.x * cameraRotationSpeed;
        // カメラの回転を適用
        _rb.MoveRotation(Quaternion.Euler(currentRotation));
        
        // カメラ縦軸
        camera.transform.Rotate(new Vector3(-_cameraInputValue.y * cameraRotationSpeed, 0, 0));
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

        _stepsAudioSource.pitch = _nowMoveSpeed / defaultMoveSpeed;
    }

    private Vector3 _hitDirection = Vector3.zero;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Damage"))
        {
            // 当たった位置から衝撃方向を計算、保存
            var playerPos = transform.position;
            var tilePos = other.transform.position;
            _hitDirection = (playerPos - tilePos).normalized;
            _hitDirection.y += RandomEx.Shared.NextFloat(0, 1);
            // ダメージイベントの発行
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

