using GTA;

namespace Euphorically.Config
{
    public class EuphoriaConfig
    {
        internal readonly ShotConfig ShotConfig;
        internal readonly ShotHeadLookConfig ShotHeadLookConfig;
        internal readonly PointGunConfig PointGunConfig;
        internal readonly ForceConfig ForceConfig;

        private const string BaseConfigName = "EuphoriaConfig";

        public EuphoriaConfig(ScriptSettings settings)
        {
            ShotConfig = new ShotConfig(settings);
            ShotHeadLookConfig = new ShotHeadLookConfig(settings);
            PointGunConfig = new PointGunConfig(settings);
            ForceConfig = new ForceConfig(settings);

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

            if (MinimumEuphoriaChance < 0f)
                MinimumEuphoriaChance = 0f;
            if (MaximumEuphoriaChance > 100f)
                MaximumEuphoriaChance = 100f;

            if (MinimumEuphoriaChance > MaximumEuphoriaChance)
                MinimumEuphoriaChance = MaximumEuphoriaChance;
            if (MaximumEuphoriaChance < MinimumEuphoriaChance)
                MaximumEuphoriaChance = MinimumEuphoriaChance;

            if (MinimumEuphoriaActiveTime > MaximumEuphoriaActiveTime)
                MinimumEuphoriaActiveTime = MaximumEuphoriaActiveTime;
            if (MaximumEuphoriaActiveTime < MinimumEuphoriaActiveTime)
                MaximumEuphoriaActiveTime = MinimumEuphoriaActiveTime;
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
    }

    internal interface IConfig
    {
        string ConfigName { get; }
    }

    internal class ShotConfig : IConfig
    {
        public string ConfigName => "ShotConfig";

        public ShotConfig(ScriptSettings settings)
        {
            UseInjuredArm = settings.GetValue(ConfigName, "UseInjuredArm", true);
            UseInjuredLeg = settings.GetValue(ConfigName, "UseInjuredLeg", true);
            UseReachForWound = settings.GetValue(ConfigName, "UseReachForWound", true);
            UseLowerLegReach = settings.GetValue(ConfigName, "UseLowerLegReach", true);
            DelayBeforeReachForWound = settings.GetValue(ConfigName, "DelayBeforeReachForWound", 0.0f);
            BodyStiffness = settings.GetValue(ConfigName, "BodyStiffness", 11f);
            ArmStiffness = settings.GetValue(ConfigName, "ArmStiffness", 10f);
            NeckStiffness = settings.GetValue(ConfigName, "NeckStiffness", 14f);
            SpineLeanMagnitude = settings.GetValue(ConfigName, "SpineLeanMagnitude", 1f);

            if (DelayBeforeReachForWound < 0f)
                DelayBeforeReachForWound = 0f;
            else if (DelayBeforeReachForWound > 10f)
                DelayBeforeReachForWound = 10f;

            if (BodyStiffness < 6f)
                BodyStiffness = 6f;
            else if (BodyStiffness > 16f)
                BodyStiffness = 16f;

            if (ArmStiffness < 6f)
                ArmStiffness = 6f;
            else if (ArmStiffness > 16f)
                ArmStiffness = 16f;

            if (NeckStiffness < 3f)
                NeckStiffness = 3f;
            else if (NeckStiffness > 16f)
                NeckStiffness = 16f;

            if (SpineLeanMagnitude < 0f)
                SpineLeanMagnitude = 0f;
        }

        public readonly bool UseInjuredArm;
        public readonly bool UseInjuredLeg;
        public readonly bool UseReachForWound;
        public readonly bool UseLowerLegReach;
        public readonly float DelayBeforeReachForWound;
        public readonly float BodyStiffness;
        public readonly float ArmStiffness;
        public readonly float NeckStiffness;
        public readonly float SpineLeanMagnitude;
    }

    internal class ShotHeadLookConfig : IConfig
    {
        public string ConfigName => "ShotHeadLookConfig";

        public ShotHeadLookConfig(ScriptSettings settings)
        {
            LookAtAttacker = settings.GetValue(ConfigName, "LookAtAttacker", true);
        }

        public readonly bool LookAtAttacker;
    }

    internal class PointGunConfig : IConfig
    {
        public string ConfigName => "PointGunConfig";

        public PointGunConfig(ScriptSettings settings)
        {
            AimAtAttacker = settings.GetValue(ConfigName, "AimAtAttacker", true);
            TurnToAttacker = settings.GetValue(ConfigName, "TurnToAttacker", true);
        }

        public readonly bool AimAtAttacker;
        public readonly bool TurnToAttacker;
    }

    internal class ForceConfig : IConfig
    {
        public string ConfigName => "ForceConfig";

        public ForceConfig(ScriptSettings settings)
        {
            UseAdditiveEuphoriaForce = settings.GetValue(ConfigName, "UseAdditiveEuphoriaForce", true);
            BaseEuphoriaForce = settings.GetValue(ConfigName, "BaseEuphoriaForce", 10.0f);
            UseRandomEuphoriaForce = settings.GetValue(ConfigName, "UseRandomEuphoriaForce", true);
            MinimumEuphoriaForce = settings.GetValue(ConfigName, "MinimumEuphoriaChance", 5.0f);
            MaximumEuphoriaForce = settings.GetValue(ConfigName, "MaximumEuphoriaChance", 15.0f);

            if (MinimumEuphoriaForce > MaximumEuphoriaForce)
                MinimumEuphoriaForce = MaximumEuphoriaForce;
            if (MaximumEuphoriaForce < MinimumEuphoriaForce)
                MaximumEuphoriaForce = MinimumEuphoriaForce;
        }

        public readonly bool UseAdditiveEuphoriaForce;
        public readonly float BaseEuphoriaForce;
        public readonly bool UseRandomEuphoriaForce;
        public readonly float MinimumEuphoriaForce;
        public readonly float MaximumEuphoriaForce;
    }
}
