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

    [SerializeField] public AudioSource clipMusicSlow, clipMusicUp, clipMusicUpBypass;

    bool flagPlayMusic = true, flagPlayMusicTime = true;

    private void Awake()
    {
        playerControl = FindFirstObjectByType<PlayerControl>();
        playerDestruction = FindFirstObjectByType<PlayerDestruction>();

        // Asignamos los listeners
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Start()
    {
        // Cargamos los valores guardados (o 1 por defecto)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Aplicamos los valores a los sliders (esto disparará SetMusicVolume y SetSFXVolume automáticamente)
        musicSlider.value = savedMusic;
        SFXSlider.value = savedSFX;

        // Por si acaso, forzamos la carga inicial al mixer
        ApplyVolume("Music", savedMusic, mixer);
        ApplyVolume("SFX", savedSFX, mixerSFX);
    }

    // Eliminamos OnDisable y guardamos directamente en las funciones
    void SetMusicVolume(float value)
    {
        ApplyVolume("Music", value, mixer);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save(); // Forzamos el guardado en Android
    }

    void SetSFXVolume(float value)
    {
        ApplyVolume("SFX", value, mixerSFX);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save(); // Forzamos el guardado en Android
    }

    // Función auxiliar para evitar repetir código y manejar el Log10(0)
    void ApplyVolume(string parameterName, float value, AudioMixer targetMixer)
    {
        // Clamp para evitar Log10 de 0 que da -Infinity
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        targetMixer.SetFloat(parameterName, volume);
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

        if (playerControl.isMusicSlow)
        {
            clipMusicSlow.mute = true;
        }
    }
}
