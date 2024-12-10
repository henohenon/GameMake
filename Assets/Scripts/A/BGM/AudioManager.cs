using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。。
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    //SE音
    [SerializeField] private AudioClip flipSound;
    [SerializeField] private AudioClip explosionSound;

    // AudioSourceを追加
    [SerializeField]
    private AudioSource audioSource;
    private void Start()
    {
        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        bgmSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        seSlider.value = seVolume;
    }
    
    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }
    
    
    public void SetSeVolume(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }
    
    public void PlayFlipCardEffect()
    {
        audioSource.PlayOneShot(flipSound);
    }
    public void PlayExplosionEffect()
    {
        audioSource.PlayOneShot(explosionSound);
    }
}