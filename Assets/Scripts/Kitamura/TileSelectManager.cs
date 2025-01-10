using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelectManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TilesManager tilesManager;
    [SerializeField] private InputActionReference openTile;
    
    // 開いている座標のリスト
    private List<Vector2> _openPositions = new()
    {
        new Vector2(0, 1),
    };
    
    private void Start()
    {
        // めくる入力の受け取り
        openTile.action.started += OpenTileCallback;
        openTile.action.Enable();
    }

    private List<TileController> _selectedTiles = new();
    public IReadOnlyList<TileController> SelectingTiles => _selectedTiles;
    public void Update()
    {
        if(_isSelectPose) return;
        // タイルの一覧がないなら実行しない
        if(tilesManager.TileControllers == null) return;
        // プレイヤーの位置の取得
        var playerPosition = playerTransform.position;
        // プレイヤーの正面の取得
        var playerForward = playerTransform.forward;
        var playerRight = playerTransform.right;

        // 既存の選択を削除
        ClearSelectedTiles();
        
        // リスト内の各座標をプレイヤーの正面方向に適用
        foreach (var offset in _openPositions)
        {
            // プレイヤーの位置 + 正面方向にリスト内のオフセットを加算
            var offsetPosition = playerPosition + playerForward * offset.y + playerRight * offset.x;
            // タイルの座標を取得
            var highLightPosition = tilesManager.GetMapPosition(offsetPosition);
            
            // タイルの座標からidを取得
            var positionTileId = MapTileCalc.GetTileId(highLightPosition, tilesManager.MapInfo.MapLength);

            // タイルあるが場合
            if (positionTileId != -1)
            {
                var tile = tilesManager.TileControllers[positionTileId];
                tile.Select();
                _selectedTiles.Add(tile);
            }
        }
    }

    private void ClearSelectedTiles()
    {
        foreach (var selectedTile in _selectedTiles)
        {
            selectedTile.Select(false);
        }
        _selectedTiles.Clear();
    }

    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // 選択してるタイルがあるなら開く
            foreach (var tile in _selectedTiles)
            {
                tile.Open();
            }
        }
    }

    private bool _isSelectPose = true;
    public void SelectPose(bool isPose)
    {
        _isSelectPose = isPose;

        if (isPose)
        {
            ClearSelectedTiles();
        }

        if (isPose)
        {
            openTile.action.Disable();
        }
        else
        {
            openTile.action.Enable();
        }
    }

    public void OpenFrontLine()
    {
        if (_selectedTiles.Count != 0)
        {
            var playerPosition = tilesManager.GetMapPosition(playerTransform.position);// プレイヤーの正面の取得
            var playerDirection = playerTransform.forward;
            // yを0にしてからノーマライズすることで、y軸を省いた上から見たときの2次元座標の正面の向きにする
            playerDirection.y = 0;
            playerDirection.Normalize();
            var playerFront = playerTransform.position + playerDirection;
            var frontPosition = tilesManager.GetMapPosition(playerFront);
            var frontTileId = MapTileCalc.GetTileId(frontPosition, tilesManager.MapInfo.MapLength);
            
            var difference = frontPosition - playerPosition;
            var currentPosition = frontPosition;
            var currentTile = tilesManager.TileControllers[frontTileId];
            while (true)
            {
                currentTile.Open();
                
                // 一つ先のタイルを取得
                currentPosition += difference;
                var currentId = MapTileCalc.GetTileId(currentPosition, tilesManager.MapInfo.MapLength);
                // 一つ先がなければ終了
                if(currentId == -1) break;
                currentTile = tilesManager.TileControllers[currentId];
            }
        }
    }

    private void OnDisable()
    {
        openTile.action.Disable();
    }
}
