using Awaken.TG.Main.Heroes;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using DefaultBowCrosshair;
using System;

namespace DefaultBowCrossair;

[HarmonyPatch(typeof(VHeroHUD), "OnMainHandChanged")]
public class WeaponChangePatch
{
    [HarmonyPostfix]
    public static void Postfix(VHeroHUD __instance)
    {
        Transform container = __instance.transform.Find("Content/Crosshair");
        if (container == null) return;

        var defaultPart = container.Find("DefaultCrosshairPart");
        if (defaultPart != null) defaultPart.gameObject.SetActive(true);

        // HideBowCrosshair config
        if (Plugin.PluginConfig.HideBowCrosshair.Value)
        {
            Plugin.Log.LogInfo("Hiding bow crosshair part due to weapon change.");
            var bowPart = container.Find("BowCrosshairPart");
            if (bowPart != null) bowPart.gameObject.SetActive(false);
        }
    }
}
