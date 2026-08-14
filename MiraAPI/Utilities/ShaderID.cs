using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MiraAPI.Utilities;

/// <summary>
/// Utility class for caching shader property IDs.
/// </summary>
public static class ShaderID
{
    private static readonly Dictionary<string, int> Cache = [];

    public static int Get(string name)
    {
        if (Cache.TryGetValue(name, out var id))
        {
            return id;
        }

        id = Shader.PropertyToID(name);
        Cache[name] = id;
        return id;
    }

    // For player shader
    public static readonly int BodyColor = Get("_BodyColor");
    public static readonly int BackColor = Get("_BackColor");
    public static readonly int VisorColor = Get("_VisorColor");

    // Main texture, very obviously used in any shader with a texture
    public static readonly int MainTex = Get("_MainTex");

    // Any masking stuff, like MeetingHud bubbles
    public static readonly int Mask = Get("_Mask");
    public static readonly int MaskComp = Get("_MaskComp");
    public static readonly int MaskLayer = Get("_MaskLayer");
    public static readonly int Stencil = Get("_Stencil");
    public static readonly int StencilComp = Get("_StencilComp");

    // Used in many tasks
    public static readonly int Color = Get("_Color");

    // Has 2 uses in the game, provided for convenience
    public static readonly int Opacity = Get("_Opacity");

    // Used once in CooldownHelpers, once in PowerBarMining
    public static readonly int NormalizedUvs = Get("_NormalizedUvs");

    // Used in many consoles
    public static readonly int Outline = Get("_Outline");
    public static readonly int OutlineColor = Get("_OutlineColor");

    // Used in some consoles
    public static readonly int AddColor = Get("_AddColor");

    // Has some uses in various locations
    public static readonly int Percent = Get("_Percent");
    public static readonly int PercentY = Get("_PercentY");
    public static readonly int Desat = Get("_Desat");

    // Used in LightSource
    public static readonly int PlayerRadius = Get("_PlayerRadius");
    public static readonly int LightRadius = Get("_LightRadius");
    public static readonly int LightOffset = Get("_LightOffset");
    public static readonly int FlashlightSize = Get("_FlashlightSize");
    public static readonly int FlashlightAngle = Get("_FlashlightAngle");

    // Used once for LightSourceGpuRenderer
    public static readonly int DepthCompressionValue = Get("_DepthCompressionValue");

    // Used in ProgressTracker
    public static readonly int Buckets = Get("_Buckets");
    public static readonly int FullBuckets = Get("_FullBuckets");

    // Used once in IntroCutscene and EndGameManager
    public static readonly int Rad = Get("_Rad");

    // Used only once for Quick Chat
    public static readonly int FaceColor = Get("_FaceColor");

    // Used for NavigationMinigame
    public static readonly int CrossHair = Get("_CrossHair");
    public static readonly int CrossColor = Get("_CrossColor");

    // Used for both SurveillanceMinigame (Planet and Normal)
    public static readonly int Center = Get("_Center");
    public static readonly int Color2 = Get("_Color2");

    // Used in ReactorShipRoom
    public static readonly int Speed = Get("_Speed");

    // Used only once in CourseMinigame
    public static readonly int AltTex = Get("_AltTex");
    public static readonly int Perc = Get("_Perc");

    // Used once in TextMarquee
    public static readonly int VertexOffsetX = Get("_VertexOffsetX");
    public static readonly int VertexOffsetY = Get("_VertexOffsetY");

    // Used in VertLineBehaviour
    public static readonly int Fade = Get("_Fade");
}
