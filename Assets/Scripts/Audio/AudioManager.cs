using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Default Volumes")]
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

    private const string MusicPrefKey = "Settings_MusicVolume";
    private const string SfxPrefKey = "Settings_SFXVolume";

    private AudioClip currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSavedVolumes();
        ApplyVolumes();
        DebugMusicState("Awake");
    }

    private void LoadSavedVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat(MusicPrefKey, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxPrefKey, sfxVolume);
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        Debug.Log($"[AudioManager] PlayMusic 请求: clip={clip.name}, 当前={currentMusic?.name}, volume={musicSource.volume}");

        if (currentMusic == clip && musicSource.isPlaying)
        {
            musicSource.volume = musicVolume;
            musicSource.loop = loop;

            Debug.Log($"[AudioManager] 同一首歌已在播放，重新应用音量: {clip.name}");
            DebugMusicState("PlayMusic-SameClip-ReapplyVolume");
            return;
        }

        currentMusic = clip;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();

        DebugMusicState("PlayMusic-AfterPlay");
    }

    public void StopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        currentMusic = null;

        DebugMusicState("StopMusic");
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.volume = sfxVolume;
        sfxSource.PlayOneShot(clip, volumeScale);

        Debug.Log($"[AudioManager] PlaySFX: clip={clip.name}, sfxSourceVolume={sfxSource.volume}, volumeScale={volumeScale}");
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
            musicSource.volume = musicVolume;

        PlayerPrefs.SetFloat(MusicPrefKey, musicVolume);
        PlayerPrefs.Save();

        DebugMusicState("SetMusicVolume");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        PlayerPrefs.SetFloat(SfxPrefKey, sfxVolume);
        PlayerPrefs.Save();

        Debug.Log($"[AudioManager] SetSFXVolume -> savedSfxVolume={sfxVolume}");
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void DebugMusicState(string fromTag)
    {
        if (musicSource == null)
        {
            Debug.Log($"[AudioManager] {fromTag} -> musicSource 为 null");
            return;
        }

        Debug.Log(
            $"[AudioManager] {fromTag} -> " +
            $"clip={musicSource.clip?.name}, " +
            $"sourceVolume={musicSource.volume}, " +
            $"savedMusicVolume={musicVolume}, " +
            $"isPlaying={musicSource.isPlaying}"
        );
    }
}