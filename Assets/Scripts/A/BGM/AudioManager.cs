using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。。
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //SE音
    [SerializeField] AudioClip SE1;
    [SerializeField] AudioClip SE2;

    // AudioSourceを追加
    [SerializeField]
    private AudioSource _audioSource;
    private void Start()
    {
        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        SESlider.value = seVolume;
    }
    
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }
    
    
    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }
    
    public void SetSE1()
    {
        _audioSource.PlayOneShot(SE1);
    }
    public void SetSE2()
    {
        _audioSource.PlayOneShot(SE2);
    }
}