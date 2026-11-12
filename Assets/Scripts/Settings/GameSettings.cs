using UnityEngine;

/// <summary>
/// Central place for all game settings.
/// Values are kept in static fields (so every scene can read them) and
/// stored in PlayerPrefs when Save() is called.
///
/// The settings are loaded and applied automatically when the game starts,
/// no GameObject is needed for that.
/// </summary>
public static class GameSettings
{
    // ---------- PlayerPrefs keys ----------
    const string KeyMusicVolume = "settings_music_volume";
    const string KeySfxVolume   = "settings_sfx_volume";
    const string KeyBrightness  = "settings_brightness";
    const string KeyQuality     = "settings_graphics_quality";
    const string KeyResWidth    = "settings_res_width";
    const string KeyResHeight   = "settings_res_height";
    const string KeyFullscreen  = "settings_fullscreen";

    // ---------- Defaults ----------
    public const float DefaultMusicVolume = 0.4f;
    public const float DefaultSfxVolume   = 0.7f;
    public const float DefaultBrightness  = 1f;

    /// <summary>Lowest allowed brightness, so the screen never goes fully black.</summary>
    public const float MinBrightness = 0.3f;

    // ---------- Current values ----------
    public static float MusicVolume = DefaultMusicVolume;
    public static float SfxVolume   = DefaultSfxVolume;
    public static float Brightness  = DefaultBrightness;
    /// <summary>0 = Low, 1 = High (see GraphicsQuality).</summary>
    public static int   QualityLevel = GraphicsQuality.High;
    public static int   ResolutionWidth;
    public static int   ResolutionHeight;
    public static bool  Fullscreen = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitOnGameStart()
    {
        Load();

        // Apply saved display settings only if the player has saved them before.
        GraphicsQuality.Apply(QualityLevel);
        if (PlayerPrefs.HasKey(KeyResWidth))
            ApplyResolution();

        BrightnessOverlay.Create();
    }

    /// <summary>Reads all values from PlayerPrefs (or defaults). Also used to undo unsaved changes.</summary>
    public static void Load()
    {
        MusicVolume      = PlayerPrefs.GetFloat(KeyMusicVolume, DefaultMusicVolume);
        SfxVolume        = PlayerPrefs.GetFloat(KeySfxVolume, DefaultSfxVolume);
        Brightness       = Mathf.Clamp(PlayerPrefs.GetFloat(KeyBrightness, DefaultBrightness), MinBrightness, 1f);
        QualityLevel     = PlayerPrefs.GetInt(KeyQuality, GraphicsQuality.High) == GraphicsQuality.Low ? GraphicsQuality.Low : GraphicsQuality.High;
        ResolutionWidth  = PlayerPrefs.GetInt(KeyResWidth, Screen.currentResolution.width);
        ResolutionHeight = PlayerPrefs.GetInt(KeyResHeight, Screen.currentResolution.height);
        Fullscreen       = PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;
    }

    /// <summary>Writes the current values to PlayerPrefs.</summary>
    public static void Save()
    {
        PlayerPrefs.SetFloat(KeyMusicVolume, MusicVolume);
        PlayerPrefs.SetFloat(KeySfxVolume, SfxVolume);
        PlayerPrefs.SetFloat(KeyBrightness, Brightness);
        PlayerPrefs.SetInt(KeyQuality, QualityLevel);
        PlayerPrefs.SetInt(KeyResWidth, ResolutionWidth);
        PlayerPrefs.SetInt(KeyResHeight, ResolutionHeight);
        PlayerPrefs.SetInt(KeyFullscreen, Fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Applies resolution, window mode and graphics quality.</summary>
    public static void ApplyDisplay()
    {
        GraphicsQuality.Apply(QualityLevel);
        ApplyResolution();
    }

    static void ApplyResolution()
    {
        if (ResolutionWidth <= 0 || ResolutionHeight <= 0) return;
        var mode = Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        // Note: in the Unity Editor this does nothing, it only works in a build.
        Screen.SetResolution(ResolutionWidth, ResolutionHeight, mode);
    }
}
