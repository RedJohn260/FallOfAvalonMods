using Awaken.TG.Main.Heroes;
using Awaken.TG.Main.Heroes.Combat;
using DefaultBowCrosshair;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace DefaultBowCrossair
{
    [HarmonyPatch(typeof(VCHeroRaycaster), "Update")]
    public class CrosshairRangePatch
    {
        private static TextMeshProUGUI _distanceText;
        private static GameObject _textContainer;

        [HarmonyPrefix]
        public static void Prefix(VCHeroRaycaster __instance)
        {
            __instance.npcDetectionMaxDistance = Plugin.PluginConfig.MaxDetectionRange.Value;
            __instance.waterDetectionMaxDistance = Plugin.PluginConfig.MaxDetectionRange.Value;
        }

        [HarmonyPostfix]
        public static void Postfix(VCHeroRaycaster __instance)
        {
            if (!Plugin.PluginConfig.EnableDistanceText.Value)
            {
                if (_textContainer != null) _textContainer.SetActive(false);
                return;
            }

            if (_distanceText == null) SetupDistanceText();

            if (__instance.NpcCollider != null)
            {
                float distance = Vector3.Distance(__instance.transform.position, __instance.NpcCollider.transform.position);

                if (distance >= Plugin.PluginConfig.MinDistanceToShowText.Value &&
                    distance <= Plugin.PluginConfig.MaxDetectionRange.Value)
                {
                    UpdateDistanceUI(distance);
                }
                else
                {
                    _textContainer?.SetActive(false);
                }
            }
            else
            {
                _textContainer?.SetActive(false);
            }
        }

        private static void UpdateDistanceUI(float distance)
        {
            _textContainer.SetActive(true);
            _distanceText.text = $"{distance:F1}m";

            float t = Mathf.InverseLerp(Plugin.PluginConfig.MinDistanceToShowText.Value, Plugin.PluginConfig.MaxDetectionRange.Value, distance);
            _distanceText.color = GetMultiLerpColor(t);
        }

        private static Color GetMultiLerpColor(float t)
        {
            if (t < 0.33f) return Color.Lerp(Color.red, Color.yellow, t / 0.33f);
            if (t < 0.66f) return Color.Lerp(Color.yellow, Color.green, (t - 0.33f) / 0.33f);
            return Color.Lerp(Color.green, Color.white, (t - 0.66f) / 0.34f);
        }

        private static void SetupDistanceText()
        {
            var hud = Hero.Current?.View<VHeroHUD>();
            Transform crosshairParent = hud?.transform.Find("Content/Crosshair");
            if (crosshairParent == null) return;

            _textContainer = new GameObject("CrosshairDistanceDisplay");
            _textContainer.transform.SetParent(crosshairParent, false);

            _distanceText = _textContainer.AddComponent<TextMeshProUGUI>();

            // Config for UI Styling
            _distanceText.fontSize = Plugin.PluginConfig.TextFontSize.Value;
            _distanceText.alignment = TextAlignmentOptions.Center;
            _distanceText.rectTransform.anchoredPosition = new Vector2(0, Plugin.PluginConfig.TextVerticalOffset.Value);

            // Outline + Underlay styling (optional)
            _distanceText.fontMaterial.EnableKeyword("OUTLINE_ON");
            _distanceText.outlineColor = new Color32(0, 0, 0, 255);
            _distanceText.outlineWidth = 0.2f;
            _distanceText.fontMaterial.EnableKeyword("UNDERLAY_ON");
            _distanceText.fontMaterial.SetColor("_UnderlayColor", new Color(0, 0, 0, 0.6f));

            // DO NOT assign font manually; use the default TMP font automatically
        }
    }
}
