using GTA;

namespace Euphorically.Config
{
    public class DebugConfig
    {
        private const string ConfigName = "DebugConfig";

        public DebugConfig(ScriptSettings settings)
        {
            ShowDebugNotifications = settings.GetValue(ConfigName, "ShowDebugNotifications", false);
        }

        public readonly bool ShowDebugNotifications;
    }
}
