using GTA;

namespace Euphorically.Config
{
    public class DebugConfig
    {
        private const string ConfigName = "DebugConfig";

        public DebugConfig(ScriptSettings settings)
        {
            ShowDebugNotifications = settings.GetValue(ConfigName, "ShowDebugNotifications", false);
            PedSearchRadius = settings.GetValue(ConfigName, "PedSearchRadius", 100f);
        }

        public readonly bool ShowDebugNotifications;
        public readonly float PedSearchRadius;
    }
}
