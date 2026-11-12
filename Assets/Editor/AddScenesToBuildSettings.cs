#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// <summary>
/// Makes sure MainMenu and Settings are in File > Build Profiles / Build Settings,
/// otherwise SceneManager.LoadScene("Settings") would fail.
/// Runs automatically every time the scripts are compiled.
/// </summary>
[InitializeOnLoad]
static class AddScenesToBuildSettings
{
    static readonly string[] RequiredScenes =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Settings.unity",
    };

    static AddScenesToBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        bool changed = false;

        foreach (string path in RequiredScenes)
        {
            if (!File.Exists(path)) continue;
            if (scenes.Exists(s => s.path == path)) continue;
            scenes.Add(new EditorBuildSettingsScene(path, true));
            changed = true;
        }

        if (changed) EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
