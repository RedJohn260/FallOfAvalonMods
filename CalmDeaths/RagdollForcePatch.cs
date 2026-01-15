using Awaken.TG.Main.Locations.Attachments.Elements.DeathBehaviours;
using HarmonyLib;
using UnityEngine;

namespace CalmDeaths
{
    [HarmonyPatch(typeof(DeathRagdollBehaviour), "AddForceToRagdoll")]
    public static class RagdollForcePatch
    {
        static bool Prefix(ref DeathRagdollBehaviour.EnableSetup setup)
        {
            if (!Plugin.PluginConfig.EnableRagdollForceScaling.Value)
                return true;

            float multiplier = Plugin.PluginConfig.RagdollForceMultiplier.Value;
            setup.forceMagnitude *= multiplier;
            setup.forceDirection = Vector3.ClampMagnitude(setup.forceDirection, 1f);
            //Plugin.Log.LogInfo($"Ragdoll force direction after clamp: {setup.forceDirection}, original magnitude: {setup.forceMagnitude}, multiplier: {multiplier}");

            return true;
        }
    }
}
