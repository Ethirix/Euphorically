using System;
using Euphorically.Config;
using GTA;
using GTA.Native;

namespace Euphorically.Utilities
{
    internal static class PlayerExtensions
    {
        internal static bool CanPlayerEuphoria(this Player player, EuphoriaConfig euphoriaConfig)
        {
            Random rnd = new Random();

            if (player.Character.IsInVehicle() && !Function.Call<bool>(Hash.CAN_KNOCK_PED_OFF_VEHICLE, player))
                return false;
            if (!player.Character.HasBeenDamagedByAnyWeapon() && !player.Character.HasBeenDamagedByAnyMeleeWeapon())
                return false;
            if (euphoriaConfig.BlockEuphoriaWithArmor && player.Character.Armor > 0)
                return false;
            if (!euphoriaConfig.UseRandomEuphoriaChance && rnd.NextDouble() * 100d > euphoriaConfig.BaseEuphoriaChance)
                return false;

            float deltaChance = euphoriaConfig.MaximumEuphoriaChance - euphoriaConfig.MinimumEuphoriaChance;
            double value = deltaChance * (rnd.NextDouble() * 100d);
            if (rnd.NextDouble() * 100d > value)
                return false;

            return true;
        }
    }
}
