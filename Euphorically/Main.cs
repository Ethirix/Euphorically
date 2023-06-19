using System;
using System.Linq;
using Euphorically.Config;
using Euphorically.Utilities;
using GTA;
using GTA.Native;
using GTA.NaturalMotion;
using GTA.UI;

namespace Euphorically
{
    public class Main : Script
    {
        private readonly EuphoriaConfig _euphoriaConfig;
        private readonly DebugConfig _debugConfig;
        private readonly Random _rng = new Random();
        
        private readonly Timer _euphoriaCooldownTimer;

        public Main()
        {
            _euphoriaConfig = new EuphoriaConfig(Settings);
            _debugConfig = new DebugConfig(Settings);
            _euphoriaCooldownTimer = new Timer(_euphoriaConfig.EuphoriaCooldown);

            Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!_euphoriaCooldownTimer.Completed)
            {
                _euphoriaCooldownTimer.Update(Game.LastFrameTime);
                return;
            }

            Ped player = Game.Player.Character;
            if (player.IsInVehicle() && !Function.Call<bool>(Hash.CAN_KNOCK_PED_OFF_VEHICLE, player))
                return;
            if (!player.HasBeenDamagedByAnyWeapon() && !player.HasBeenDamagedByAnyMeleeWeapon())
                return;
            if (_euphoriaConfig.BlockEuphoriaWithArmor && player.Armor > 0)
                return;

            if (_rng.NextDouble() * 100f > _euphoriaConfig.EuphoriaChance)
                return;

            Ped attackingPed = World.GetNearbyPeds(player, _debugConfig.PedSearchRadius,
                Array.Empty<Model>()).FirstOrDefault(p => player.HasBeenDamagedBy(p));

            if (attackingPed != null)
                RunEuphoriaLogic(player, attackingPed);

            player.ClearLastWeaponDamage();
        }

        private void RunEuphoriaLogic(Ped attackedPed, Ped attackingPed)
        {
            _euphoriaCooldownTimer.Restart();

            attackedPed.Euphoria.Shot.AllowInjuredArm = true;
            attackedPed.Euphoria.Shot.AllowInjuredLeg = true;
            attackedPed.Euphoria.Shot.ReachForWound = true;
            attackedPed.Euphoria.Shot.TimeBeforeReachForWound = 0.0f;

            Bone bone;
            using (OutputArgument output = new OutputArgument())
            {
                Function.Call(Hash.GET_PED_LAST_DAMAGE_BONE, attackedPed, output);
                bone = (Bone)output.GetResult<int>();
            }

            ThrowNotification($"Bone ID: {bone}");
            attackedPed.Euphoria.ShotNewBullet.BodyPart = 0;

            if (true) //LookAtAttacker
            {
                attackedPed.Euphoria.ShotHeadLook.UseHeadLook = true;
                attackedPed.Euphoria.ShotHeadLook.HeadLook = attackingPed.Position;
                attackedPed.Euphoria.ShotHeadLook.Start(5);
                //TODO: implement a bunch of config
                //TODO: continue working on euphoria
                //TODO: figure out what id each bone is in Euphoria IDs (0-21)
            }
        }

        private void ThrowNotification(string message)
        {
            if (_debugConfig.ShowDebugNotifications)
            {
                Notification.Show(message);
            }
        }
    }
}
