using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Plays background music tracks one after another. When the last track
/// finishes, it starts again from the first one.
///
/// Setup:
///  1. Add this script to a GameObject (e.g. "MusicManager").
///  2. Right-click the component header -> "Load Clips From Sounds Folder"
///     (or drag your clips into the Tracks list manually).
///  3. Press Play.
/// </summary>
public class MusicPlaylist : MonoBehaviour
{
    [Header("Playlist")]
    [Tooltip("Tracks are played in this order.")]
    public List<AudioClip> tracks = new List<AudioClip>();

    [Tooltip("Shuffle the order every time the playlist starts over.")]
    public bool shuffleEachLoop = false;

    [Tooltip("Start playing automatically when the scene loads.")]
    public bool playOnStart = true;

    [Tooltip("Keep the music playing when switching scenes.")]
    public bool persistAcrossScenes = true;

    [Header("Audio")]
    [Tooltip("Drag your BackgroundMusic AudioSource here. If empty, the AudioSource on this GameObject is used (or one is added).")]
    public AudioSource musicSource;

    // Volume now comes from GameSettings (changed in the Settings scene).

    [Tooltip("Silent gap in seconds between tracks.")]
    public float gapBetweenTracks = 0f;

    [Header("Editor helper")]
    [Tooltip("Folder used by 'Load Clips From Sounds Folder'.")]
    public string soundsFolder = "Assets/Sounds";

    [Tooltip("Clips whose names contain any of these words are skipped (sound effects).")]
    public List<string> ignoreNameContains = new List<string> { "ShakingDice" };

    private AudioSource source;
    private int currentIndex = -1;
    private bool appPaused;
    private float nextTrackTime = -1f;
    private static MusicPlaylist instance;
    private bool isDuplicate;

    /// <summary>The music player that is currently playing (null if none).</summary>
    public static MusicPlaylist Instance => instance;

    /// <summary>Called every time a new track starts. Used by the Settings scene.</summary>
    public event System.Action<AudioClip> TrackChanged;

    /// <summary>Name of the track that is playing right now.</summary>
    public string CurrentTrackName =>
        source != null && source.clip != null ? source.clip.name : "-";

    void Awake()
    {
        // Avoid duplicate music managers when persisting across scenes.
        if (persistAcrossScenes)
        {
            if (instance != null && instance != this)
            {
                isDuplicate = true;
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance == null)
        {
            instance = this;
        }

        source = musicSource != null ? musicSource : GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        source.Stop();                // stop any clip preset on the source
        source.loop = false;          // we handle looping ourselves
        source.playOnAwake = false;
        source.volume = GameSettings.MusicVolume;
    }

    void Start()
    {
        if (isDuplicate) return;
        if (playOnStart) Play();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Update()
    {
        if (isDuplicate || source == null || tracks.Count == 0) return;

        source.volume = GameSettings.MusicVolume;

        // Waiting through the gap between tracks.
        if (nextTrackTime >= 0f)
        {
            if (Time.unscaledTime >= nextTrackTime)
            {
                nextTrackTime = -1f;
                PlayNext();
            }
            return;
        }

        // Track finished (ignore the case where the app/audio is just paused).
        if (currentIndex >= 0 && !source.isPlaying && !appPaused)
        {
            if (gapBetweenTracks > 0f)
                nextTrackTime = Time.unscaledTime + gapBetweenTracks;
            else
                PlayNext();
        }
    }

    // ---------- Public controls ----------

    public void Play()
    {
        if (tracks.Count == 0)
        {
            Debug.LogWarning("MusicPlaylist: no tracks assigned.", this);
            return;
        }
        currentIndex = -1;
        if (shuffleEachLoop) Shuffle();
        PlayNext();
    }

    public void Stop()
    {
        source.Stop();
        currentIndex = -1;
        nextTrackTime = -1f;
    }

    public void Skip()
    {
        nextTrackTime = -1f;
        PlayNext();
    }

    /// <summary>Plays the next track (same as Skip).</summary>
    public void Next() => Skip();

    /// <summary>Goes back to the previous track in the list.</summary>
    public void Previous()
    {
        if (tracks.Count == 0) return;
        nextTrackTime = -1f;

        int index = currentIndex;
        int safety = tracks.Count;
        do
        {
            index = (index - 1 + tracks.Count) % tracks.Count;
        } while (tracks[index] == null && --safety > 0);

        PlayIndex(index);
    }

    /// <summary>Plays the track with the given index in the list.</summary>
    public void PlayIndex(int index)
    {
        if (index < 0 || index >= tracks.Count || tracks[index] == null) return;
        currentIndex = index;
        nextTrackTime = -1f;
        source.clip = tracks[currentIndex];
        source.Play();
        TrackChanged?.Invoke(source.clip);
    }

    // ---------- Internals ----------

    private void PlayNext()
    {
        if (tracks.Count == 0) return;

        currentIndex++;

        // Reached the end of the list: loop back to the beginning.
        if (currentIndex >= tracks.Count)
        {
            currentIndex = 0;
            if (shuffleEachLoop) Shuffle();
        }

        // Skip empty slots safely.
        int safety = tracks.Count;
        while (tracks[currentIndex] == null && safety-- > 0)
            currentIndex = (currentIndex + 1) % tracks.Count;

        if (tracks[currentIndex] == null) return;

        source.clip = tracks[currentIndex];
        source.Play();
        TrackChanged?.Invoke(source.clip);
        Debug.Log($"MusicPlaylist: now playing \"{source.clip.name}\" ({currentIndex + 1}/{tracks.Count})");
    }

    private void Shuffle()
    {
        for (int i = tracks.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (tracks[i], tracks[j]) = (tracks[j], tracks[i]);
        }
    }

    void OnApplicationPause(bool paused) { appPaused = paused; }
    void OnApplicationFocus(bool focused) { if (focused) appPaused = false; }

#if UNITY_EDITOR
    [ContextMenu("Load Clips From Sounds Folder")]
    private void LoadClipsFromFolder()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { soundsFolder });
        var found = new List<AudioClip>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null) continue;

            bool ignored = false;
            foreach (string word in ignoreNameContains)
            {
                if (!string.IsNullOrEmpty(word) &&
                    clip.name.IndexOf(word, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ignored = true;
                    break;
                }
            }
            if (!ignored) found.Add(clip);
        }

        found.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));

        Undo.RecordObject(this, "Load music clips");
        tracks = found;
        EditorUtility.SetDirty(this);
        Debug.Log($"MusicPlaylist: loaded {found.Count} clip(s) from {soundsFolder}.", this);
    }
#endif
}
