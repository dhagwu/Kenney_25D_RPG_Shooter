using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Mixer Parameter Names")]
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private const string MusicPrefKey = "Settings_MusicVolume";
    private const string SfxPrefKey = "Settings_SFXVolume";

    private void Start()
    {
        float musicValue = PlayerPrefs.GetFloat(MusicPrefKey, 1f);
        float sfxValue = PlayerPrefs.GetFloat(SfxPrefKey, 1f);

        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.SetValueWithoutNotify(musicValue);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.SetValueWithoutNotify(sfxValue);
        }

        ApplyMusicVolume(musicValue);
        ApplySFXVolume(sfxValue);
    }

    public void OnMusicSliderChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MusicPrefKey, value);
        PlayerPrefs.Save();
    }

    public void OnSFXSliderChanged(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SfxPrefKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float normalizedValue)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParameter, NormalizedToDb(normalizedValue));
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(normalizedValue);
        }
    }

    private void ApplySFXVolume(float normalizedValue)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParameter, NormalizedToDb(normalizedValue));
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(normalizedValue);
        }
    }

    private float NormalizedToDb(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }
}