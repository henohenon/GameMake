using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BombTileController : TileController
{
    [SerializeField] private GameObject bombObject;
    [SerializeField] private Collider damageCollider; 
    
    public override bool Open()
    {
        var baseResult =  base.Open();
        if (baseResult)
        {
            Explosion();
            bombObject.SetActive(true);
        }

        return baseResult;
    }

    private async void Explosion()
    {
        await UniTask.WaitForSeconds(0.3f);
        if(damageCollider) damageCollider.enabled = true;

        await UniTask.WaitForSeconds(0.1f);
        if(damageCollider) damageCollider.enabled = false;
    }
}
