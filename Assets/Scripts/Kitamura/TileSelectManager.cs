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
        openTile.action.started += OpenTileCallback;
        openTile.action.Enable();
    }

    private TileController _selectingTile;
    public void Update()
    {
        if(tilesManager.TileControllers == null) return;
        var playerPosition = playerTransform.position;
        var playerDirection = playerTransform.forward;
        playerDirection.y = 0;
        playerDirection.Normalize();
        var playerForwardPosition = playerPosition + playerDirection;
        
        var highLightPosition = tilesManager.GetMapPosition(playerForwardPosition);
        
        var positionTileId = MapTileCalc.GetTileId(highLightPosition, tilesManager.MapInfo.MapLength);

        if (positionTileId == -1)
        {
            if (_selectingTile)
            {
                _selectingTile.Select(false);
                _selectingTile = null;
            }
            return;
        }
        
        var tile = tilesManager.TileControllers[positionTileId];

        if (tile != _selectingTile)
        {
            _selectingTile?.Select(false);
            _selectingTile = tile;
            _selectingTile.Select();
        }
    }
    
    
    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(_selectingTile)
            {
                Debug.Log("Open");
                _selectingTile.Open();
            }
        }
    }

}
