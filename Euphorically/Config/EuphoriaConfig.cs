using GTA;

namespace Euphorically.Config
{
    public class EuphoriaConfig
    {
        private const string ConfigName = "EuphoriaConfig";

        public EuphoriaConfig(ScriptSettings settings)
        {
            BlockEuphoriaWithArmor = settings.GetValue(ConfigName, "BlockEuphoriaWithArmour", true);
            EuphoriaChance = settings.GetValue(ConfigName, "EuphoriaChance", 10.0f);
            EuphoriaCooldown = settings.GetValue(ConfigName, "EuphoriaCooldown", 5.0f);
        }

        public readonly bool BlockEuphoriaWithArmor;
        public readonly float EuphoriaChance;
        public readonly float EuphoriaCooldown;
    }
}
