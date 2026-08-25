using UnityEngine;
using UnityEngine.Audio;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioMixer audioMixer;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string FullscreenKey = "Fullscreen";
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";

    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public bool IsFullscreen { get; private set; }
    public bool IsOpen => settingsPanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        IsFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        ApplyMusicVolume();
        ApplySfxVolume();
        Screen.fullScreen = IsFullscreen;

        if (PlayerPrefs.HasKey(ResolutionWidthKey))
        {
            int width = PlayerPrefs.GetInt(ResolutionWidthKey);
            int height = PlayerPrefs.GetInt(ResolutionHeightKey);
            Screen.SetResolution(width, height, IsFullscreen);
        }

        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetMusicVolume(float value01)
    {
        MusicVolume = Mathf.Clamp01(value01);
        ApplyMusicVolume();
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
    }

    public void SetSfxVolume(float value01)
    {
        SfxVolume = Mathf.Clamp01(value01);
        ApplySfxVolume();
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
    }

    private void ApplyMusicVolume() => audioMixer.SetFloat("MusicVolume", ToDecibels(MusicVolume));
    private void ApplySfxVolume() => audioMixer.SetFloat("SFXVolume", ToDecibels(SfxVolume));

    private static float ToDecibels(float value01) => Mathf.Log10(Mathf.Max(value01, 0.0001f)) * 20f;

    public void SetFullscreen(bool value)
    {
        IsFullscreen = value;
        Screen.fullScreen = value;
        PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, IsFullscreen);
        PlayerPrefs.SetInt(ResolutionWidthKey, width);
        PlayerPrefs.SetInt(ResolutionHeightKey, height);
    }
}
