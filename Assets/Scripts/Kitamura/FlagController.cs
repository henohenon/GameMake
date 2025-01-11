using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlagController : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        var startPos = transform.position;
        startPos.y += 1;
        LMotion.Create(startPos, transform.localPosition, 0.5f).WithEase(Ease.OutExpo).BindToLocalPosition(transform);
        _audioSource = GetComponent<AudioSource>();
        
        _audioSource.Play();
    }

    public async void Remove()
    {
        LMotion.Create(transform.localScale, Vector3.zero, 0.5f).WithEase(Ease.OutExpo).BindToLocalScale(transform);
        LMotion.Create(transform.localPosition, Vector3.zero, 0.5f).WithEase(Ease.OutExpo).BindToLocalPosition(transform);

        await PlayRemoveSound();
        Destroy(this.gameObject);
    }

    private async UniTask PlayRemoveSound()
    {
        _audioSource.Stop();
        _audioSource.pitch = -1.0f;
        _audioSource.loop = true;
        _audioSource.Play();
        await UniTask.WaitForSeconds(0.7f);
        _audioSource.Stop();
    }
}
