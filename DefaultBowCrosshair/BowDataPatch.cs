using HarmonyLib;
using Awaken.TG.Main.Heroes.Crosshair;

namespace DefaultBowCrossair;

[HarmonyPatch(typeof(BowCrosshairPart))]
public class BowDataPatch
{
    // prevents it from taking over the crosshair slot
    [HarmonyPatch("Layer", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool PrefixLayer(ref CrosshairLayer __result)
    {
        // same layer as the Default crosshair
        __result = CrosshairLayer.OverridingLayer0;
        return false;
    }

    // makes sure it never outranks the Default crosshair
    [HarmonyPatch("Priority", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool PrefixPriority(ref int __result)
    {
        __result = 0;
        return false;
    }
}
