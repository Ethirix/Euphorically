using System;
using System.Collections.Generic;
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
            
            Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, euphoriaTime, euphoriaTime, 1, 1, 1, 0);

            InitializeShotConfig(in attackedPed);
            
            HandlePedBones(in attackedPed);

            InitializeShotHeadLookConfig(attackedPed, attackingPed);

            if (_euphoriaConfig.PointGunConfig.AimAtAttacker)
            {
                attackedPed.Euphoria.PointGun.UseTurnToTarget = _euphoriaConfig.PointGunConfig.TurnToAttacker;
                attackedPed.Euphoria.PointGun.RightHandTarget = attackingPed.Position;
                ThrowNotification(
                    $"Attacking Ped Pos: {attackingPed.Position} - Offset: {new Vector3(attackedPed.Position.X - attackingPed.Position.X, attackedPed.Position.Y - attackingPed.Position.Y, attackedPed.Position.Z - attackingPed.Position.Z)}");
                attackedPed.Euphoria.PointGun.Start();
            }

            ApplyForceToRagdoll(in attackedPed, in attackingPed);
            
            attackedPed.Euphoria.Shot.Start();
            attackedPed.Euphoria.ShotNewBullet.Start();
            attackedPed.Euphoria.ShotSnap.Start();
        }

        private void HandlePedBones(in Ped ped)
        {
            Bone bone;
            using (OutputArgument output = new OutputArgument())
            {
                Function.Call(Hash.GET_PED_LAST_DAMAGE_BONE, ped, output);
                bone = (Bone)output.GetResult<int>();
            }

            EuphoriaBones? euphoriaBone = bone.ConvertToEuphoriaBone();
            if (euphoriaBone != null)
                ped.Euphoria.ShotNewBullet.BodyPart = (int)euphoriaBone;
            else
                ped.Euphoria.Shot.ReachForWound = false;
        }

        private void ApplyForceToRagdoll(in Ped attackedPed, in Ped attackingPed)
        {
            Vector3 distanceNormalized = new Vector3(attackedPed.Position.X - attackingPed.Position.X,
                attackedPed.Position.Y - attackingPed.Position.Y, attackedPed.Position.Z - attackingPed.Position.Z).Normalized;

            float force = _euphoriaConfig.BaseEuphoriaForce;
            if (_euphoriaConfig.UseRandomEuphoriaForce)
            {
                float deltaChance = _euphoriaConfig.MaximumEuphoriaForce - _euphoriaConfig.MinimumEuphoriaForce;
                force = _euphoriaConfig.MinimumEuphoriaForce + deltaChance * (float)(_rnd.NextDouble() * 100d);
            }

            attackedPed.ApplyForce(distanceNormalized * force, Vector3.Zero, ForceType.MaxForceRot);
        }

        private void InitializeShotConfig(in Ped ped)
        {
            ped.Euphoria.Shot.AllowInjuredArm = _euphoriaConfig.ShotConfig.UseInjuredArm;
            ped.Euphoria.Shot.AllowInjuredLeg = _euphoriaConfig.ShotConfig.UseInjuredLeg;
            ped.Euphoria.Shot.AllowInjuredLowerLegReach = _euphoriaConfig.ShotConfig.UseLowerLegReach;
            ped.Euphoria.Shot.ReachForWound = _euphoriaConfig.ShotConfig.UseReachForWound;
            ped.Euphoria.Shot.BodyStiffness = _euphoriaConfig.ShotConfig.BodyStiffness;
            ped.Euphoria.Shot.ArmStiffness = _euphoriaConfig.ShotConfig.ArmStiffness;
            ped.Euphoria.Shot.NeckStiffness = _euphoriaConfig.ShotConfig.NeckStiffness;
            ped.Euphoria.Shot.CpainMag = _euphoriaConfig.ShotConfig.SpineLeanMagnitude;
            ped.Euphoria.Shot.TimeBeforeReachForWound = _euphoriaConfig.ShotConfig.DelayBeforeReachForWound;
        }

        private void InitializeShotHeadLookConfig(in Ped attackedPed, in Ped attackingPed)
        {
            if (_euphoriaConfig.ShotHeadLookConfig.LookAtAttacker)
            {
                attackedPed.Euphoria.ShotHeadLook.UseHeadLook = true;
                attackedPed.Euphoria.ShotHeadLook.HeadLook = attackingPed.Position;
                attackedPed.Euphoria.ShotHeadLook.Start();
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