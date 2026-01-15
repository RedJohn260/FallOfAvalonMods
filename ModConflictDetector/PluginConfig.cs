using BepInEx.Configuration;
using UnityEngine;

namespace ModConflictDetector
{
    public class PluginConfig
    {
        public ConfigEntry<bool> EnablePopup;
        public ConfigEntry<bool> EnableDetailedLogging;
        public ConfigEntry<KeyCode> ToggleKey; // New: Key to open window manually

        public PluginConfig(ConfigFile config)
        {
            config.SaveOnConfigSet = false;
            try
            {
                // If this is false, window only opens for Serious conflicts
                EnablePopup = config.Bind("General", "EnablePopup", false, "Show the in-game UI when conflicts are detected.");

                // If this is true, we print to LogOutput.txt
                EnableDetailedLogging = config.Bind("General", "EnableDetailedLogging", true, "Log detailed conflict information to the BepInEx console.");

                // Key to manually toggle the window (Default F7)
                ToggleKey = config.Bind("General", "ToggleKey", KeyCode.F5, "Key to manually open/close the conflict window.");
            }
            finally
            {
                config.Save();
                config.SaveOnConfigSet = true;
            }
        }
    }
}
