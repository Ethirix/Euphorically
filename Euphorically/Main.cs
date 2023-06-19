using System;
using System.Linq;
using System.Windows.Forms;
using Euphorically.Config;
using Euphorically.Utilities;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.NaturalMotion;
using GTA.UI;
using Timer = Euphorically.Utilities.Timer;

namespace Euphorically
{
    public class Main : Script
    {
        private readonly EuphoriaConfig _euphoriaConfig;
        private readonly DebugConfig _debugConfig;
        private readonly Random _rnd = new Random();
   
        private readonly Timer _euphoriaCooldownTimer;

        public Main()
        {
            _euphoriaConfig = new EuphoriaConfig(Settings);
            _debugConfig = new DebugConfig(Settings);
            _euphoriaCooldownTimer = new Timer(_euphoriaConfig.EuphoriaCooldown);

            Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnTick(object sender, EventArgs e)
        {
            Player player = Game.Player;
            Ped character = player.Character;
            
            if (!_euphoriaCooldownTimer.Completed)
            {
                _euphoriaCooldownTimer.Update(Game.LastFrameTime);
                character.ClearLastWeaponDamage();
                return;
            }

            if (!player.CanPlayerEuphoria(_euphoriaConfig))
                return;

            Ped attackingPed = World.GetNearbyPeds(character, _debugConfig.PedSearchRadius,
                Array.Empty<Model>()).FirstOrDefault(p => character.HasBeenDamagedBy(p));

            if (attackingPed != null)
                RunEuphoriaLogic(character, attackingPed);

            character.ClearLastWeaponDamage();
        }

        private void RunEuphoriaLogic(Ped attackedPed, Ped attackingPed)
        {
            int euphoriaTime = _euphoriaConfig.UseRandomEuphoriaActiveTime
                ? _rnd.Next(_euphoriaConfig.MinimumEuphoriaActiveTime, _euphoriaConfig.MaximumEuphoriaActiveTime + 1)
                : _euphoriaConfig.BaseEuphoriaActiveTime;
            
            ThrowNotification($"Timer Started: {_euphoriaConfig.EuphoriaCooldown + euphoriaTime}");
            _euphoriaCooldownTimer.Restart(_euphoriaConfig.EuphoriaCooldown + euphoriaTime);
            euphoriaTime *= 1000;
            
            Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, 4000, 5000, 1, 1, 1, 0);
            Function.Call(Hash.CREATE_NM_MESSAGE, true, 0);
            Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);

            //attackedPed.Euphoria.Shot.AllowInjuredArm = _euphoriaConfig.UseInjuredArm;
            Function.Call(Hash.CREATE_NM_MESSAGE, _euphoriaConfig.UseInjuredArm, 1024);
            Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);

            //attackedPed.Euphoria.Shot.AllowInjuredLeg = _euphoriaConfig.UseInjuredLeg;
            Function.Call(Hash.CREATE_NM_MESSAGE, _euphoriaConfig.UseInjuredLeg, 1025);
            Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);

            attackedPed.Euphoria.Shot.ReachForWound = _euphoriaConfig.UseReachForWound;
            attackedPed.Euphoria.Shot.BodyStiffness = 16f;
            attackedPed.Euphoria.Shot.ArmStiffness = 16f;
            attackedPed.Euphoria.Shot.NeckStiffness = 16f;
            attackedPed.Euphoria.Shot.CpainMag = 0.1f;
            attackedPed.Euphoria.Shot.TimeBeforeReachForWound = _euphoriaConfig.DelayBeforeReachForWound;

            Bone bone;
            using (OutputArgument output = new OutputArgument())
            {
                Function.Call(Hash.GET_PED_LAST_DAMAGE_BONE, attackedPed, output);
                bone = (Bone)output.GetResult<int>();
            }

            ThrowNotification($"Bone ID: {bone}");
            attackedPed.Euphoria.ShotNewBullet.BodyPart = 4;

            //4 might be 

            if (_euphoriaConfig.LookAtAttacker)
            {
                attackedPed.Euphoria.ShotHeadLook.UseHeadLook = true;
                attackedPed.Euphoria.ShotHeadLook.HeadLook = attackingPed.Position;
                attackedPed.Euphoria.ShotHeadLook.Start(euphoriaTime);
                //TODO: continue working on euphoria
                //TODO: figure out what id each bone is in Euphoria IDs (0-21)
            }

            if (_euphoriaConfig.AimAtAttacker)
            {
                attackedPed.Euphoria.PointGun.UseTurnToTarget = _euphoriaConfig.TurnToAttacker;
                attackedPed.Euphoria.PointGun.RightHandTarget = attackingPed.Position;
                attackedPed.Euphoria.PointGun.Start(euphoriaTime);
            }

            Vector3 distanceNormalized = new Vector3(attackedPed.Position.X - attackingPed.Position.X,
                attackedPed.Position.Y - attackingPed.Position.Y, 0f).Normalized;


            float force = _euphoriaConfig.BaseEuphoriaForce;
            if (_euphoriaConfig.UseRandomEuphoriaForce)
            {
                float deltaChance = _euphoriaConfig.MaximumEuphoriaForce - _euphoriaConfig.MinimumEuphoriaForce;
                force = _euphoriaConfig.MinimumEuphoriaForce + deltaChance * (float)(_rnd.NextDouble() * 100d);
            }

            attackedPed.ApplyForce(distanceNormalized * force);
            attackedPed.Euphoria.Shot.Start(euphoriaTime);
            attackedPed.Euphoria.ShotNewBullet.Start(euphoriaTime);
            attackedPed.Euphoria.ShotSnap.Start(euphoriaTime);
        }

        private void ThrowNotification(string message)
        {
            if (_debugConfig.ShowDebugNotifications)
            {
                Notification.Show(message);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs key)
        {
            if (key.KeyCode == Keys.Subtract)
            {
                Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, 4000, 5000, 1, 1, 1, 0);
                Function.Call(Hash.CREATE_NM_MESSAGE, true, 0);
                Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);
                Function.Call(Hash.CREATE_NM_MESSAGE, true, 1151);
                Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);
                Function.Call(Hash.CREATE_NM_MESSAGE, true, 372);
                Function.Call(Hash.GIVE_PED_NM_MESSAGE, Game.Player.Character);
            }
        }
    }
}