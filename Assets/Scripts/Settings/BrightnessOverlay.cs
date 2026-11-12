using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Custom feature: screen brightness.
/// A full-screen black image is drawn on top of everything. The lower the
/// brightness setting, the less transparent the image is.
/// It survives scene changes, so the brightness is the same in every scene.
/// Created automatically by GameSettings when the game starts.
/// </summary>
public class BrightnessOverlay : MonoBehaviour
{
    static BrightnessOverlay instance;
    Image overlay;

    public static void Create()
    {
        if (instance != null) return;

        var go = new GameObject("BrightnessOverlay");
        DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32000; // above every other canvas

        var imageGo = new GameObject("Dim");
        imageGo.transform.SetParent(go.transform, false);
        var rect = imageGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        instance = go.AddComponent<BrightnessOverlay>();
        instance.overlay = imageGo.AddComponent<Image>();
        instance.overlay.raycastTarget = false; // never blocks clicks
        instance.Refresh();
    }

    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        float alpha = 1f - Mathf.Clamp(GameSettings.Brightness, GameSettings.MinBrightness, 1f);
        overlay.color = new Color(0f, 0f, 0f, alpha);
        overlay.enabled = alpha > 0.001f;
    }
}
