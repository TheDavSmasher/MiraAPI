using System.Linq;
using HarmonyLib;
using MiraAPI.Utilities.Assets;
using TMPro;

namespace MiraAPI.Patches;
[HarmonyPatch]
public static class TmpAwakePatch
{
    [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.Awake))]
    [HarmonyPostfix]
    public static void TmpAwakePostfix(TextMeshPro __instance)
    {
        if (!TmpSpriteUtils.AssetHolder || !__instance.m_spriteAsset)
        {
            return;
        }

        /*
        if (__instance.m_spriteAsset.name == "RoleIcons")
        {
            Error($"Stencil Comp: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ID_StencilComp)}");
            Error($"Stencil ID: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ID_StencilID)}");
            Error($"Stencil Op: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ID_StencilOp)}");
            Error($"Stencil Write: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ID_StencilWriteMask)}");
            Error($"Stencil Read: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ID_StencilReadMask)}");
            Error($"Cull Mode: {__instance.m_spriteAsset.material.GetFloat(ShaderUtilities.ShaderTag_CullMode)}");
        }*/
        __instance.m_spriteAsset.fallbackSpriteAssets.Add(TmpSpriteUtils.AssetHolder);
        __instance.UpdateMeshPadding();
    }
}
