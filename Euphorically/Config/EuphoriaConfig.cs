using GTA;

namespace Euphorically.Config
{
    public class EuphoriaConfig
    {
        private const string ConfigName = "EuphoriaConfig";

        public EuphoriaConfig(ScriptSettings settings)
        {
            EuphoriaFromMeleeDamage = settings.GetValue(ConfigName, "EuphoriaFromMeleeDamage", true);
            RandomizeEuphoriaTypes = settings.GetValue(ConfigName, "RandomizeEuphoriaTypes", true);
            EuphoriaType = settings.GetValue(ConfigName, "EuphoriaType",  EuphoriaType.Normal);
            RandomizeEuphoriaTimeRange = settings.GetValue(ConfigName, "RandomizeEuphoriaTimeRange", true);
            EuphoriaMinimumTime = settings.GetValue(ConfigName, "EuphoriaMinimumTime", 2);
            EuphoriaMaximumTime = settings.GetValue(ConfigName, "EuphoriaMaximumTime", 4);
            RandomizeEuphoriaChance = settings.GetValue(ConfigName, "RandomizeEuphoriaChance", true);
            EuphoriaMinimumChance = settings.GetValue(ConfigName, "EuphoriaMinimumChance", 10);
            EuphoriaMaximumChance = settings.GetValue(ConfigName, "EuphoriaMaximumChance", 20);
            EuphoriaAntiSpamTime = settings.GetValue(ConfigName, "EuphoriaAntiSpamTime", 3f);

            if (EuphoriaMaximumChance < EuphoriaMinimumChance)
            {
                EuphoriaMaximumChance = EuphoriaMinimumChance;
            }

            if (EuphoriaMaximumTime < EuphoriaMinimumTime)
            {
                EuphoriaMaximumTime = EuphoriaMinimumTime;
            }
        }

        public readonly bool EuphoriaFromMeleeDamage;
        public readonly bool RandomizeEuphoriaTypes;
        public readonly EuphoriaType EuphoriaType;
        public readonly bool RandomizeEuphoriaTimeRange;
        public readonly int EuphoriaMinimumTime;
        public readonly int EuphoriaMaximumTime;
        public readonly bool RandomizeEuphoriaChance;
        public readonly int EuphoriaMinimumChance;
        public readonly int EuphoriaMaximumChance;
        public readonly float EuphoriaAntiSpamTime;
    }
}
