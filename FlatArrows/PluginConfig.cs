using BepInEx.Configuration;

namespace FlatArrows
{
    public class PluginConfig
    {
        public ConfigEntry<bool> EnableFlatArrowsMod { get; private set; }
        public ConfigEntry<float> ArrowSpeedMultiplier { get; private set; }
        public ConfigEntry<float> VerticalCompensation { get; private set; }
        public ConfigEntry<float> BowPullSpeedMultiplier { get; private set; }
        public ConfigEntry<float> BowReleaseSpeedMultiplier { get; private set; }

        public PluginConfig(ConfigFile config)
        {
            config.SaveOnConfigSet = false;
            try
            {
                EnableFlatArrowsMod = config.Bind("General","EnableFlatArrows",true,"Toggle flat arrow flight.");

                ArrowSpeedMultiplier = config.Bind("General","ArrowSpeedMultiplier",2.0f,
                    new ConfigDescription("Multiply arrow speed.",
                    new AcceptableValueRange<float>(0.1f, 5f)));

                VerticalCompensation = config.Bind("General","VerticalCompensation",0.02f,
                    new ConfigDescription("Small upward bias to compensate gravity fall-off.",
                    new AcceptableValueRange<float>(0.00f, 0.1f)));

                BowPullSpeedMultiplier = config.Bind("General","BowPullSpeedMultiplier",2.0f,
                    new ConfigDescription("Multiplier for bow pull speed",
                    new AcceptableValueRange<float>(0.1f, 5f)));

                BowReleaseSpeedMultiplier = config.Bind("General","BowReleaseSpeedMultiplier",2.0f,
                    new ConfigDescription("Multiplier for bow release speed",
                    new AcceptableValueRange<float>(0.1f, 5f)));
            }
            finally
            {
                config.Save();
                config.SaveOnConfigSet = true;
            }
        }
    }
}
