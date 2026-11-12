using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Low / High graphics quality for the URP project.
///
/// Low  = the game is rendered at half resolution (bigger pixels) and then
///        stretched to the screen, no anti-aliasing (MSAA) and no shadows.
///        Fewer pixels to draw = better performance on weak computers.
/// High = the original settings of the URP asset.
///
/// Note: Screen Space - Overlay UI (menus) is always drawn at full resolution,
/// so the difference is visible in the 3D / camera image, not on UI buttons.
///
/// The URP asset is a project file, so the original values are restored when
/// the game closes / Play mode stops, otherwise the Editor would keep "Low".
/// </summary>
public static class GraphicsQuality
{
    public const int Low = 0;
    public const int High = 1;

    /// <summary>Render scale used for Low (0.5 = half the pixels in each direction).</summary>
    public const float LowRenderScale = 0.5f;

    static UniversalRenderPipelineAsset urp;
    static bool originalSaved;
    static float originalRenderScale;
    static int originalMsaa;
    static float originalShadowDistance;

    public static string Name(int level) => level == Low ? "Low" : "High";

    public static void Apply(int level)
    {
        if (!SaveOriginal())
        {
            Debug.LogWarning("GraphicsQuality: the project does not use a URP asset, quality was not changed.");
            return;
        }

        if (level == Low)
        {
            urp.renderScale = LowRenderScale;
            urp.msaaSampleCount = 1;      // anti-aliasing off
            urp.shadowDistance = 0f;      // shadows off
        }
        else
        {
            RestoreOriginal();
        }
    }

    static bool SaveOriginal()
    {
        var current = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (current == null) return false;

        if (!originalSaved || current != urp)
        {
            if (originalSaved) RestoreOriginal(); // restore the old asset before switching
            urp = current;
            originalRenderScale = urp.renderScale;
            originalMsaa = urp.msaaSampleCount;
            originalShadowDistance = urp.shadowDistance;
            originalSaved = true;
            Application.quitting -= RestoreOriginal;
            Application.quitting += RestoreOriginal;
        }
        return true;
    }

    static void RestoreOriginal()
    {
        if (!originalSaved || urp == null) return;
        urp.renderScale = originalRenderScale;
        urp.msaaSampleCount = originalMsaa;
        urp.shadowDistance = originalShadowDistance;
    }
}
