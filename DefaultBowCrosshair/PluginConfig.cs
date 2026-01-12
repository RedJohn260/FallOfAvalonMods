using BepInEx.Configuration;
using UnityEngine;

namespace DefaultBowCrosshair;

public class PluginConfig
{
    // General Toggles
    public ConfigEntry<bool> HideBowCrosshair { get; private set; }
    public ConfigEntry<bool> EnableDistanceText { get; private set; }
    public ConfigEntry<bool> EnableCustomCrosshairColors { get; private set; }

    // Range Settings
    public ConfigEntry<float> MaxDetectionRange { get; private set; }
    public ConfigEntry<float> MinDistanceToShowText { get; private set; }

    // UI Appearance
    public ConfigEntry<float> TextFontSize { get; private set; }
    public ConfigEntry<float> TextVerticalOffset { get; private set; }

    // Crosshair Colors
    public ConfigEntry<Color> HostileColor { get; private set; }
    public ConfigEntry<Color> NonHostileColor { get; private set; }
    public ConfigEntry<Color> DefaultColor { get; private set; }

    public PluginConfig(ConfigFile config)
    {
        config.SaveOnConfigSet = false;
        try
        {
            // General
            HideBowCrosshair = config.Bind("1. General", "Hide Bow Crosshair", true, "If true, the bow-specific lines will be hidden.");
            EnableDistanceText = config.Bind("1. General", "Enable Distance Text", true, "Shows the numerical distance to NPCs above the crosshair.");
            EnableCustomCrosshairColors = config.Bind("1. General", "Enable Custom Colors", true, "Allows the crosshair to change color based on target type.");

            // Range
            MaxDetectionRange = config.Bind("2. Range", "Max Detection Distance", 100f,
                new ConfigDescription("The maximum distance the raycast will detect targets (standard is 50).",
                new AcceptableValueRange<float>(1.0f, 500f)));

            MinDistanceToShowText = config.Bind("2. Range", "Minimum Text Distance", 5f,
                new ConfigDescription("Distance text hides if the target is closer than this. Do not go below 1.",
                new AcceptableValueRange<float>(1.0f, 499f)));

            // UI
            TextFontSize = config.Bind("3. UI", "Font Size", 12f,
                new ConfigDescription("The size of the distance text.",
                new AcceptableValueRange<float>(1.0f, 120f)));

            TextVerticalOffset = config.Bind("3. UI", "Vertical Offset", 65f,
                new ConfigDescription("How high above the crosshair the text appears.",
                new AcceptableValueRange<float>(-200f, 200f)));

            // Colors
            HostileColor = config.Bind("4. Colors", "Hostile Target Color", Color.red, "Color when aiming at enemies.");
            NonHostileColor = config.Bind("4. Colors", "Non-Hostile Target Color", Color.green, "Color when aiming at friendlies.");
            DefaultColor = config.Bind("4. Colors", "Default Color", Color.white, "Standard crosshair color.");
        }
        finally
        {
            config.Save();
            config.SaveOnConfigSet = true;
        }
    }
}
