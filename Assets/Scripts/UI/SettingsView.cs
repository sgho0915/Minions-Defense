// SettingsView.cs
using System;
using UnityEngine;
using UnityEngine.UI;

// BGM, SFX 볼륨 조절 View, 슬라이더 값 SoundManager와 동기화
public class SettingsView : UIView
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    public event Action OnHideComplete;

    protected override void Awake()
    {
        base.Awake();

        // 슬라이더 값 변경 시마다 SoundManager 볼륨 설정 함수 호출
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);

        closeButton.onClick.AddListener(() => { 
            SoundManager.Instance.PlayButtonClicked(); 
            GlobalUIManager.Instance.PopView();
        });
    }

    private void Start()
    {
        // View 활성화 시 playerprefs에 저장된 볼륨값으로 슬라이더 초기화
        InitializeSliderValues();
    }

    private void InitializeSliderValues()
    {
        bgmSlider.value = PlayerPrefs.GetFloat(SoundManager.BGM_VOLUME_PARAM, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SoundManager.SFX_VOLUME_PARAM, 1f);
    }
}
