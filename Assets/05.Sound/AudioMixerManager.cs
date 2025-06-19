using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (bgmSlider != null)
        {
            float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
            bgmSlider.value = bgm;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = sfx;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", VolumeToDecibel(volume));
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", VolumeToDecibel(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }


    private float VolumeToDecibel(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
    }
}
