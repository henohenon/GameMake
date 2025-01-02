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
    
    private MapInfo _mapInfo;

    private void Start()
    {
        openTile.action.started += OpenTileCallback;
        openTile.action.Enable();
    }

    private TileController selectingTile;
    public void Update()
    {
        var playerPosition = playerTransform.position;
        var playerDirection = playerTransform.forward;
        playerDirection.y = 0;
        playerDirection.Normalize();
        var playerForwardPosition = playerPosition + playerDirection;
        
        var highLightPosition = tilesManager.GetMapPosition(playerForwardPosition);
        
        var positionTileId = MapTileCalc.GetTileId(highLightPosition, _mapInfo.Width, _mapInfo.Height);

        if (positionTileId == -1)
        {
            if (selectingTile)
            {
                selectingTile.Select(false);
                selectingTile = null;
            }
            return;
        }
        
        var tile = tilesManager.TileControllers[positionTileId];

        if (tile != selectingTile)
        {
            selectingTile?.Select(false);
            selectingTile = tile;
            selectingTile.Select();
        }
    }
    
    
    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(selectingTile)
            {
                Debug.Log("Open");
                selectingTile.Open();
            }
        }
    }

}
