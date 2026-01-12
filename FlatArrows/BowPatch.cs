using Awaken.TG.Main.Animations.FSM.Heroes.Machines;
using Awaken.TG.Main.Animations.FSM.Heroes.States.Bow;
using Awaken.TG.Main.Cameras.CameraStack;
using Awaken.TG.MVC;
using HarmonyLib;
using UnityEngine;

namespace FlatArrows
{
    [HarmonyPatch(typeof(BowFSM), "CalculateArrowVelocity")]
    public class BowPatch
    {
        static bool Prefix(Vector3 firePoint, float magnitude, ref Vector3 __result)
        {
            if (!Plugin.PluginConfig.EnableFlatArrowsMod.Value)
                return true; // run original if disabled

            Vector3 forward = Vector3.forward;

            if (firePoint != Vector3.zero)
            {
                forward = (firePoint - Vector3.zero).normalized; 
            }

            var camTransform = World.Only<CameraStateStack>()?.MainCamera?.transform;
            if (camTransform != null)
            {
                forward = camTransform.forward.normalized;
            }

            float speedMult = Plugin.PluginConfig.ArrowSpeedMultiplier.Value;
            float vertical = Plugin.PluginConfig.VerticalCompensation.Value;

            __result = (forward + Vector3.up * vertical) * magnitude * speedMult;

            //Plugin.Log.LogInfo($"[FlatArrows] Velocity Result: {__result}");

            return false; // skip original
        }
    }

    [HarmonyPatch(typeof(BowPull), "AttackSpeed", MethodType.Getter)]
    class PatchBowPullSpeed
    {
        static void Postfix(ref float __result)
        {
            if (!Plugin.PluginConfig.EnableFlatArrowsMod.Value) return;

            __result *= Plugin.PluginConfig.BowPullSpeedMultiplier.Value;
            Plugin.Log.LogInfo($"[FlatArrows] Bow Pull Speed Modified: {__result}");
        }
    }

    [HarmonyPatch(typeof(BowRelease), "AttackSpeed", MethodType.Getter)]
    class PatchBowReleaseSpeed
    {
        static void Postfix(ref float __result)
        {
            if (!Plugin.PluginConfig.EnableFlatArrowsMod.Value) return;

            __result *= Plugin.PluginConfig.BowReleaseSpeedMultiplier.Value;
            Plugin.Log.LogInfo($"[FlatArrows] Bow Release Speed Modified: {__result}");
        }
    }
}
