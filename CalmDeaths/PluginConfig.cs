using BepInEx.Configuration;

namespace CalmDeaths;

public class PluginConfig
{
    // TODO: add config if needed
    // public ConfigEntry<bool> MyValue { get; private set; }
    public ConfigEntry<bool> EnableRagdollForceScaling { get; private set; }
    public ConfigEntry<float> RagdollForceMultiplier { get; private set; }

    public PluginConfig(ConfigFile config)
    {
        config.SaveOnConfigSet = false;
        try
        {
            // TODO: add config bindings as needed
            // MyValue = config.Bind("Category", "ValueName", DefaultValue, "Description");
            EnableRagdollForceScaling = config.Bind("General", "Enabled", true);
            RagdollForceMultiplier = config.Bind("General", "RagdollForceMultiplier", 0.15f,
                new ConfigDescription("Scales ragdoll impulse force on death",
                new AcceptableValueRange<float>(0.0f, 1.0f)));
        }
        finally
        {
            config.Save();
            config.SaveOnConfigSet = true;
        }
    }
}
