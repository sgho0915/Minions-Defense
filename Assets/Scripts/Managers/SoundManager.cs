// Soundmanager.cs
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 게임의 BGM, SFX를 관리하는 DontDestroyOnLoad 싱글톤
/// AudioMixer를 통해 볼률제어, PlayerPrefs로 설정 저장 및 로드
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip lobbyBGM;
    [SerializeField] private AudioClip stageBGM;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip stageSelectClip;

    // AudioMixer에 설정된 파라미터 이름
    public const string BGM_VOLUME_PARAM = "BGMVolume";
    public const string SFX_VOLUME_PARAM = "SFXVolume";

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 저장된 볼륨 설정 로드, 없으면 기본값 사용
        LoadVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_PARAM, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_PARAM, 1f);

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    /// <summary>
    /// BGM 볼륨 설정(0.0001 ~ 1.0)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_PARAM, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SFX 볼륨 설정(0.0001 ~ 1.0)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_PARAM, volume);
        PlayerPrefs.Save();
    }


    public void PlayBGM(string sceneName)
    {
        AudioClip clip = null;

        if (sceneName.StartsWith("Stage_"))
            clip = stageBGM;
        else if(sceneName.StartsWith("Lobby"))
            clip = lobbyBGM;

        if(clip != null && bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }        
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 AudioClip이 null입니다.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClicked()
    {
        sfxSource.PlayOneShot(buttonClickClip);
    }

    public void StageSelectedClicked()
    {
        sfxSource.PlayOneShot(stageSelectClip);
    }
}
