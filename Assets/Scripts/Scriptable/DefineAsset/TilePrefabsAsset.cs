using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

// なんか好きだから使ってるけど別にScriptableである必要はない
[CreateAssetMenu(fileName = "TilePrefabsAsset", menuName = "TilePrefabsData")]
public class TilePrefabsAsset : ScriptableObject
{
    [AssetsOnly]
    public TileController safeTilePrefab;
    [AssetsOnly]
    public TileController bombTilePrefab;
    [AssetsOnly]
    public BlueItemTileController blueItemTilePrefab;
    [AssetsOnly]
    public TileController yellowItemTilePrefab;
    [AssetsOnly]
    public TileController purpleItemTilePrefab;
}
