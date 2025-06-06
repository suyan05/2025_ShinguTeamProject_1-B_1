using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider     masterSlider;
    //[SerializeField] private Slider     sfxSlider;
    //[SerializeField] private Slider     bgmslider;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        //sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        //bgmslider.onValueChanged.AddListener(SetMusicVolume);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
    //public void SetSFXVolume(float volume)
    //{
    //    audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    //}
    //public void SetMusicVolume(float volume)
    //{
    //    audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    //}
}