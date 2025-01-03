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

    public TileController SelectingTile { get; private set; }
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
            if (SelectingTile)
            {
                SelectingTile.Select(false);
                SelectingTile = null;
            }
            return;
        }
        
        var tile = tilesManager.TileControllers[positionTileId];

        if (tile != SelectingTile)
        {
            SelectingTile?.Select(false);
            SelectingTile = tile;
            SelectingTile.Select();
        }
    }
    
    private void OpenTileCallback(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(SelectingTile)
            {
                Debug.Log("Open");
                SelectingTile.Open();
            }
        }
    }
}
