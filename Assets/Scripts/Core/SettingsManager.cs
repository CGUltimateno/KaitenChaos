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
    public int CurrentWidth { get; private set; }
    public int CurrentHeight { get; private set; }
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 720;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        IsFullscreen = PlayerPrefs.GetInt(FullscreenKey, 0) == 1;

        int width = PlayerPrefs.GetInt(ResolutionWidthKey, DefaultWidth);
        int height = PlayerPrefs.GetInt(ResolutionHeightKey, DefaultHeight);
        ApplyMusicVolume();
        ApplySfxVolume();
        Screen.fullScreen = IsFullscreen;

        Screen.SetResolution(width, height, IsFullscreen);
        CurrentWidth = width;
        CurrentHeight = height;

        settingsPanel.SetActive(false);
    }

    private void Start()
    {
        ApplyMusicVolume();
        ApplySfxVolume();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        PauseManager.Instance.SetBackgroundBlocking(false);
        foreach (var handle in settingsPanel.GetComponentsInChildren<VolumeSliderHandle>(true))
            handle.Refresh();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        PauseManager.Instance.SetBackgroundBlocking(true);
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
        PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);

        if (value)
        {
            Resolution native = Screen.currentResolution;
            Screen.SetResolution(native.width, native.height, true);
            CurrentWidth = native.width;
            CurrentHeight = native.height;
        }
        else
        {
            int width = PlayerPrefs.GetInt(ResolutionWidthKey, DefaultWidth);
            int height = PlayerPrefs.GetInt(ResolutionHeightKey, DefaultHeight);
            Screen.SetResolution(width, height, false);
            CurrentWidth = width;
            CurrentHeight = height;
        }
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, IsFullscreen);
        PlayerPrefs.SetInt(ResolutionWidthKey, width);
        PlayerPrefs.SetInt(ResolutionHeightKey, height);
        CurrentWidth = width;
        CurrentHeight = height;
    }
}
