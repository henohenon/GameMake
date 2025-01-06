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
    
    private void Start()
    {
        // めくる入力の受け取り
        openTile.action.started += OpenTileCallback;
        openTile.action.Enable();
    }

    public TileController SelectingTile { get; private set; }
    public void Update()
    {
        // タイルの一覧がないなら実行しない
        if(tilesManager.TileControllers == null) return;
        // プレイヤーの位置の取得
        var playerPosition = playerTransform.position;
        // プレイヤーの正面の取得
        var playerDirection = playerTransform.forward;
        // yを0にしてからノーマライズすることで、y軸を省いた上から見たときの2次元座標の正面の向きにする
        playerDirection.y = 0;
        playerDirection.Normalize();
        // プレイヤーの位置+正面方向に1の位置を取得
        var playerForwardPosition = playerPosition + playerDirection;
        
        // タイルの座標を取得
        var highLightPosition = tilesManager.GetMapPosition(playerForwardPosition);
        
        // タイルの座標からidを取得
        var positionTileId = MapTileCalc.GetTileId(highLightPosition, tilesManager.MapInfo.MapLength);

        // タイルがない場合
        if (positionTileId == -1)
        {
            // 今選択してるものがあれば
            if (SelectingTile)
            {
                // セレクトを切る
                SelectingTile.Select(false);
                // 選択を無くす
                SelectingTile = null;
            }
            // 終了
            return;
        }
        
        var tile = tilesManager.TileControllers[positionTileId];
        //　既存の選択されているタイルと違えば
        if (tile != SelectingTile)
        {
            // 既存の選択解除
            SelectingTile?.Select(false);
            // 新規選択
            SelectingTile = tile;
            SelectingTile.Select();
        }
    }
    
    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // 選択してるタイルがあるなら開く
            if(SelectingTile)
            {
                Debug.Log("Open");
                SelectingTile.Open();
            }
        }
    }
}
