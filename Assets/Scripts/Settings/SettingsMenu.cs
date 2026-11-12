using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logic of the Settings scene.
///
/// - Volume and brightness change immediately so the player can hear / see the result.
/// - Resolution, window mode and quality are applied only after "Apply".
/// - Nothing is saved until "Apply" is pressed. "Back" without Apply undoes the changes.
/// - Music tracks are switched immediately (that is not a setting that needs saving).
///
/// All UI events (OnClick / OnValueChanged) are connected in the Inspector.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public Slider musicSlider;
    public TMP_Text musicValueText;
    public Slider sfxSlider;
    public TMP_Text sfxValueText;
    public TMP_Text trackText;

    [Header("Display")]
    public TMP_Text resolutionText;
    public TMP_Text displayModeText;
    public TMP_Text qualityText;

    [Header("Custom feature: brightness")]
    public Slider brightnessSlider;
    public TMP_Text brightnessValueText;

    [Header("Other")]
    public TMP_Text statusText;
    public SoundEffectsScript soundEffects;
    public SceneChangerScript sceneChanger;
    public string mainMenuScene = "MainMenu";

    // Common resolutions, used when the system does not report a list (e.g. in the Editor).
    static readonly Vector2Int[] FallbackResolutions =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1366, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160),
    };

    readonly List<Vector2Int> resolutions = new List<Vector2Int>();
    int resolutionIndex;

    // Values that are currently saved – used to know if something changed.
    float savedMusic, savedSfx, savedBrightness;
    int savedQuality, savedWidth, savedHeight;
    bool savedFullscreen;

    float lastSfxPreviewTime;
    float statusHideTime = -1f;
    MusicPlaylist subscribedPlaylist;

    void Start()
    {
        GameSettings.Load();
        RememberSavedValues();
        BuildResolutionList();

        musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
        brightnessSlider.minValue = GameSettings.MinBrightness;
        brightnessSlider.maxValue = 1f;
        brightnessSlider.SetValueWithoutNotify(GameSettings.Brightness);

        subscribedPlaylist = MusicPlaylist.Instance;
        if (subscribedPlaylist != null) subscribedPlaylist.TrackChanged += OnTrackChanged;

        RefreshAll();
        SetStatus("");
    }

    void OnDestroy()
    {
        if (subscribedPlaylist != null) subscribedPlaylist.TrackChanged -= OnTrackChanged;
    }

    void Update()
    {
        if (statusHideTime > 0f && Time.unscaledTime > statusHideTime)
        {
            statusHideTime = -1f;
            SetStatus("");
        }
    }

    // ================= UI events =================

    // 2.1 Music volume
    public void OnMusicVolumeChanged(float value)
    {
        GameSettings.MusicVolume = value;
        RefreshAll();
    }

    // 2.2 Sound effect volume
    public void OnSfxVolumeChanged(float value)
    {
        GameSettings.SfxVolume = value;

        // Play a short sound so the player hears the new volume.
        if (soundEffects != null && Time.unscaledTime - lastSfxPreviewTime > 0.25f)
        {
            lastSfxPreviewTime = Time.unscaledTime;
            soundEffects.Click();
        }
        RefreshAll();
    }

    // 2.3 Switch music track manually
    public void PreviousTrack()
    {
        if (MusicPlaylist.Instance != null) MusicPlaylist.Instance.Previous();
        RefreshTrack();
    }

    public void NextTrack()
    {
        if (MusicPlaylist.Instance != null) MusicPlaylist.Instance.Next();
        RefreshTrack();
    }

    // 2.4 Resolution
    public void PreviousResolution() => ChangeResolution(-1);
    public void NextResolution() => ChangeResolution(1);

    // 2.4 Window mode (part of the resolution settings)
    public void ToggleDisplayMode()
    {
        GameSettings.Fullscreen = !GameSettings.Fullscreen;
        RefreshAll();
    }

    // 2.5 Quality
    public void PreviousQuality() => ChangeQuality(-1);
    public void NextQuality() => ChangeQuality(1);

    // 2.6 Custom feature – brightness
    public void OnBrightnessChanged(float value)
    {
        GameSettings.Brightness = value;
        RefreshAll();
    }

    // 2.7 Apply changes
    public void ApplyChanges()
    {
        GameSettings.ApplyDisplay();
        GameSettings.Save();
        RememberSavedValues();
        RefreshAll();
        SetStatus("Settings saved!", 2f);
    }

    // 2.7 Back to main menu (unsaved changes are undone)
    public void BackToMenu()
    {
        if (HasUnsavedChanges()) GameSettings.Load();

        if (sceneChanger != null) sceneChanger.LoadScene(mainMenuScene);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
    }

    // ================= Helpers =================

    void ChangeResolution(int step)
    {
        if (resolutions.Count == 0) return;
        resolutionIndex = (resolutionIndex + step + resolutions.Count) % resolutions.Count;
        GameSettings.ResolutionWidth = resolutions[resolutionIndex].x;
        GameSettings.ResolutionHeight = resolutions[resolutionIndex].y;
        RefreshAll();
    }

    // Goes through the levels from Project Settings > Quality (e.g. Low, High).
    void ChangeQuality(int step)
    {
        int count = QualitySettings.names.Length;
        if (count == 0) return;
        GameSettings.QualityLevel = (GameSettings.QualityLevel + step + count) % count;
        RefreshAll();
    }

    void BuildResolutionList()
    {
        resolutions.Clear();
        foreach (Resolution r in Screen.resolutions)
        {
            var size = new Vector2Int(r.width, r.height);
            if (!resolutions.Contains(size)) resolutions.Add(size);
        }

        if (resolutions.Count < 2)
        {
            foreach (var size in FallbackResolutions)
                if (!resolutions.Contains(size)) resolutions.Add(size);
        }

        var current = new Vector2Int(GameSettings.ResolutionWidth, GameSettings.ResolutionHeight);
        if (current.x > 0 && current.y > 0 && !resolutions.Contains(current)) resolutions.Add(current);

        resolutions.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
        resolutionIndex = Mathf.Max(0, resolutions.IndexOf(current));
    }

    void RememberSavedValues()
    {
        savedMusic = GameSettings.MusicVolume;
        savedSfx = GameSettings.SfxVolume;
        savedBrightness = GameSettings.Brightness;
        savedQuality = GameSettings.QualityLevel;
        savedWidth = GameSettings.ResolutionWidth;
        savedHeight = GameSettings.ResolutionHeight;
        savedFullscreen = GameSettings.Fullscreen;
    }

    bool HasUnsavedChanges()
    {
        return !Mathf.Approximately(savedMusic, GameSettings.MusicVolume)
            || !Mathf.Approximately(savedSfx, GameSettings.SfxVolume)
            || !Mathf.Approximately(savedBrightness, GameSettings.Brightness)
            || savedQuality != GameSettings.QualityLevel
            || savedWidth != GameSettings.ResolutionWidth
            || savedHeight != GameSettings.ResolutionHeight
            || savedFullscreen != GameSettings.Fullscreen;
    }

    void OnTrackChanged(AudioClip clip) => RefreshTrack();

    void RefreshAll()
    {
        musicValueText.text = Percent(GameSettings.MusicVolume);
        sfxValueText.text = Percent(GameSettings.SfxVolume);
        brightnessValueText.text = Percent(GameSettings.Brightness);

        resolutionText.text = $"{GameSettings.ResolutionWidth} x {GameSettings.ResolutionHeight}";
        displayModeText.text = GameSettings.Fullscreen ? "Fullscreen" : "Windowed";

        string[] names = QualitySettings.names;
        qualityText.text = GameSettings.QualityLevel >= 0 && GameSettings.QualityLevel < names.Length
            ? names[GameSettings.QualityLevel]
            : "-";

        RefreshTrack();

        if (statusHideTime < 0f)
            SetStatus(HasUnsavedChanges() ? "You have unsaved changes" : "");
    }

    void RefreshTrack()
    {
        trackText.text = MusicPlaylist.Instance != null ? MusicPlaylist.Instance.CurrentTrackName : "No music";
    }

    void SetStatus(string message, float hideAfterSeconds = 0f)
    {
        if (statusText != null) statusText.text = message;
        statusHideTime = hideAfterSeconds > 0f ? Time.unscaledTime + hideAfterSeconds : -1f;
    }

    static string Percent(float value) => Mathf.RoundToInt(value * 100f) + "%";
}
