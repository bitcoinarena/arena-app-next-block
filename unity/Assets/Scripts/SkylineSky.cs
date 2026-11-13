using UnityEngine;

/// <summary>
/// The sky of every view: the website's header image all the way round, in place of Unity's default sky
/// (kanban #968). Presentation only: nothing here reaches the core.
/// </summary>
public static class SkylineSky
{
    /// <summary>The fog's colour, so the world's edge melts into the horizon and not into a pale band.</summary>
    public static readonly Color Horizon = new Color(0.42f, 0.18f, 0.24f);

    public static bool Install()
    {
        var shader = Resources.Load<Shader>("SkylineSky");
        var panorama = Resources.Load<Texture2D>("skyline-panorama");
        if (shader == null || panorama == null || !shader.isSupported) return false;
        var sky = new Material(shader);
        sky.SetTexture("_MainTex", panorama);
        RenderSettings.skybox = sky;
        RenderSettings.fogColor = Horizon;
        return true;
    }
}
