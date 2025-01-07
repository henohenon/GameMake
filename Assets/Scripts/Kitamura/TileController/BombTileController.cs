using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class BombTileController : TileController
{
    [SerializeField] private GameObject bombObject;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private AudioClip alertAudio;
    [SerializeField] private AudioClip explosionAudio;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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
        if (_audioSource)
        {
            _audioSource.clip = alertAudio;
            _audioSource.loop = true;
            _audioSource.time = 0.25f;
            _audioSource.pitch = 2;
            _audioSource.Play();
        }
        await UniTask.WaitForSeconds(0.5f);
        if(damageCollider) damageCollider.enabled = true;
        if (_audioSource)
        {
            _audioSource.clip = explosionAudio;
            _audioSource.loop = false;
            _audioSource.pitch = 1;
            _audioSource.Play();
        }

        await UniTask.WaitForSeconds(0.1f);
        if(damageCollider) damageCollider.enabled = false;
    }
}
