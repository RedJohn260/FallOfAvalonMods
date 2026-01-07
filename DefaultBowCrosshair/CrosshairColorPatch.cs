using System.Reflection;
using Awaken.TG.Main.Heroes.Crosshair;
using DefaultBowCrosshair;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultBowCrossair;

[HarmonyPatch]
public class CrosshairColorPatch
{
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() => AccessTools.Method(typeof(VCrosshairPart<DefaultCrosshairPart>), "ChangeColors");

    [HarmonyPrefix]
    public static bool Prefix(object __instance, CrosshairTargetType type)
    {
        // Skip entirely if custom colors are disabled in config
        if (!Plugin.PluginConfig.EnableCustomCrosshairColors.Value) return true;

        var img = Traverse.Create(__instance).Field("colorableCrosshairImage").GetValue<Image>();
        if (img == null) return true;

        if (type == CrosshairTargetType.Hostile)
            img.color = Plugin.PluginConfig.HostileColor.Value;
        else if (type == CrosshairTargetType.NonHostile)
            img.color = Plugin.PluginConfig.NonHostileColor.Value;
        else
            img.color = Plugin.PluginConfig.DefaultColor.Value;

        return false;
    }
}
