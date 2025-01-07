using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoldierGameSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip gameClearSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    [Button]
    public void PlayClearSound()
    {
        _audioSource.clip = gameClearSound;
        _audioSource.Play();
    }
}
