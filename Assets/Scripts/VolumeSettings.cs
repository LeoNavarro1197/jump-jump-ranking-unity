using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer, mixerSFX;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;

    PlayerControl playerControl;
    PlayerDestruction playerDestruction;

    [SerializeField] AudioSource clipMusicSlow, clipMusicUp, clipMusicUpBypass;

    bool flagPlayMusic = true, flagPlayMusicTime = true;

    private void Awake()
    {
        playerControl = FindFirstObjectByType<PlayerControl>();
        playerDestruction = FindFirstObjectByType<PlayerDestruction>();

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void SetMusicVolume(float value)
    {
        mixer.SetFloat("Music", Mathf.Log10(value) * 20); 
    }

    void SetSFXVolume(float value)
    {
        mixerSFX.SetFloat("SFX", Mathf.Log10(value) * 20);
    }

    void Update()
    {
        if (playerControl.START && flagPlayMusic)
        {
            clipMusicSlow.mute = true;
            clipMusicUp.mute = false;
            clipMusicUpBypass.mute = true;
            flagPlayMusic = false;

            if (flagPlayMusicTime)
            {
                clipMusicUp.time = 0f;
                clipMusicUpBypass.time = 0f;
                flagPlayMusicTime = false;
            }
        }

        if (playerDestruction.DEATH)
        {
            clipMusicUpBypass.mute = false;
            clipMusicUp.mute = true;
            flagPlayMusic = true;
        }

        // Music Slow Apagada
        if (playerControl.isMusicSlow)
        {
            clipMusicSlow.mute = true;
        }
    }
}
