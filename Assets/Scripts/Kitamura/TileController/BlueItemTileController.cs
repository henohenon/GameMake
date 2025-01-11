using R3;
using RandomExtensions;
using Scriptable;
using UnityEngine;

public class BlueItemTileController : TileController
{
    [SerializeField] private MeshRenderer iconRenderer;

    public void SetItemIcon(Sprite itemIcon)
    {
        // SpriteからTexture2Dを取得する
        Texture2D texture = itemIcon.texture;
        iconRenderer.material.mainTexture = texture;
        
        // テクスチャの幅と高さを取得
        float width = texture.width;
        float height = texture.height;

        // オブジェクトのスケールを調整
        Vector3 newScale = iconRenderer.transform.localScale;

        if (width > height)
        {
            // 横長の場合、横を基準にスケールを調整
            newScale.y = newScale.x * (height / width);
        }
        else
        {
            // 縦長の場合、縦を基準にスケールを調整
            newScale.x = newScale.y * (width / height);
        }

        // スケールを適用
        iconRenderer.transform.localScale = newScale;
   }

    public override bool Open()
    {
        var baseResult = base.Open();
        if (baseResult)
        {
            iconRenderer.gameObject.SetActive(true);
        }

        return baseResult;
    }
}
