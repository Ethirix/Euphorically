using GTA;

namespace Euphorically.Config
{
    public class EuphoriaConfig
    {
        private const string BaseConfigName = "EuphoriaConfig";
        private const string PlayerConfigName = "PlayerEuphoriaConfig";

        public EuphoriaConfig(ScriptSettings settings)
        {
            BlockEuphoriaWithArmor = settings.GetValue(BaseConfigName, "BlockEuphoriaWithArmour", true);
            EuphoriaCooldown = settings.GetValue(BaseConfigName, "EuphoriaCooldown", 5.0f);

            BaseEuphoriaChance = settings.GetValue(BaseConfigName, "BaseEuphoriaChance", 10.0f);
            UseRandomEuphoriaChance = settings.GetValue(BaseConfigName, "UseRandomEuphoriaChance", true);
            MinimumEuphoriaChance = settings.GetValue(BaseConfigName, "MinimumEuphoriaChance", 5.0f);
            MaximumEuphoriaChance = settings.GetValue(BaseConfigName, "MaximumEuphoriaChance", 15.0f);

            BaseEuphoriaActiveTime = settings.GetValue(BaseConfigName, "BaseEuphoriaActiveTime", 3);
            UseRandomEuphoriaActiveTime = settings.GetValue(BaseConfigName, "UseRandomEuphoriaActiveTime", true);
            MinimumEuphoriaActiveTime = settings.GetValue(BaseConfigName, "MinimumEuphoriaActiveTime", 2);
            MaximumEuphoriaActiveTime = settings.GetValue(BaseConfigName, "MaximumEuphoriaActiveTime", 4);

            UseInjuredArm = settings.GetValue(PlayerConfigName, "UseInjuredArm", true);
            UseInjuredLeg = settings.GetValue(PlayerConfigName, "UseInjuredLeg", true);
            UseReachForWound = settings.GetValue(PlayerConfigName, "UseReachForWound", true);
            DelayBeforeReachForWound = settings.GetValue(PlayerConfigName, "DelayBeforeReachForWound", 0.1f);
            LookAtAttacker = settings.GetValue(PlayerConfigName, "LookAtAttacker", true);
            AimAtAttacker = settings.GetValue(PlayerConfigName, "AimAtAttacker", true);
            TurnToAttacker = settings.GetValue(PlayerConfigName, "TurnToAttacker", true);

            BaseEuphoriaForce = settings.GetValue(PlayerConfigName, "BaseEuphoriaForce", 10.0f);
            UseRandomEuphoriaForce = settings.GetValue(PlayerConfigName, "UseRandomEuphoriaForce", true);
            MinimumEuphoriaChance = settings.GetValue(PlayerConfigName, "MinimumEuphoriaChance", 5.0f);
            MaximumEuphoriaChance = settings.GetValue(PlayerConfigName, "MaximumEuphoriaChance", 15.0f);

            if (MinimumEuphoriaChance > MaximumEuphoriaChance)
                MinimumEuphoriaChance = MaximumEuphoriaChance;
            if (MaximumEuphoriaChance < MinimumEuphoriaChance)
                MaximumEuphoriaChance = MinimumEuphoriaChance;

            if (MinimumEuphoriaActiveTime > MaximumEuphoriaActiveTime)
                MinimumEuphoriaActiveTime = MaximumEuphoriaActiveTime;
            if (MaximumEuphoriaActiveTime < MinimumEuphoriaActiveTime)
                MaximumEuphoriaActiveTime = MinimumEuphoriaActiveTime;

            if (MinimumEuphoriaForce > MaximumEuphoriaForce)
                MinimumEuphoriaForce = MaximumEuphoriaForce;
            if (MaximumEuphoriaForce < MinimumEuphoriaForce)
                MaximumEuphoriaForce = MinimumEuphoriaForce;
        }

        public readonly bool BlockEuphoriaWithArmor;
        public readonly float EuphoriaCooldown;

        public readonly float BaseEuphoriaChance;
        public readonly bool UseRandomEuphoriaChance;
        public readonly float MinimumEuphoriaChance;
        public readonly float MaximumEuphoriaChance;

        public readonly int BaseEuphoriaActiveTime;
        public readonly bool UseRandomEuphoriaActiveTime;
        public readonly int MinimumEuphoriaActiveTime;
        public readonly int MaximumEuphoriaActiveTime;

        public readonly bool UseInjuredArm;
        public readonly bool UseInjuredLeg;
        public readonly bool UseReachForWound;
        public readonly float DelayBeforeReachForWound;
        public readonly bool LookAtAttacker;
        public readonly bool AimAtAttacker;
        public readonly bool TurnToAttacker;

        public readonly float BaseEuphoriaForce;
        public readonly bool UseRandomEuphoriaForce;
        public readonly float MinimumEuphoriaForce;
        public readonly float MaximumEuphoriaForce;
    }
}
