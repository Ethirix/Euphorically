using System;
using System.Linq;
using System.Windows.Forms;
using Euphorically.Config;
using Euphorically.Utilities;
using GTA;
using GTA.Math;
using GTA.Native;
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

        private EuphoriaData _euphoriaData;

        private bool _cancelledRagdoll = true;
        private bool _cancelCheck = false;
        private Vector3 _posLastFrame;
        private float _frameTime;

        public Main()
        {
            _euphoriaConfig = new EuphoriaConfig(Settings);
            _debugConfig = new DebugConfig(Settings);
            _euphoriaCooldownTimer = new Timer(0);

            Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

#if DEBUG
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Subtract:
                    Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, 10000, 10000, 1, 1, 1, 0);
                    
                    InitializeShotConfig(Game.Player.Character);
                    
                    //500 for shotgun
                    Game.Player.Character.Euphoria.ApplyBulletImpulse.Impulse = -Game.Player.Character.ForwardVector * 200;
                    Game.Player.Character.Euphoria.ApplyBulletImpulse.HitPoint = Game.Player.Character.Position;
                    
                    Game.Player.Character.Euphoria.Shot.Start();
                    Game.Player.Character.Euphoria.ShotNewBullet.Start();
                    Game.Player.Character.Euphoria.ShotSnap.Start();
                    Game.Player.Character.Euphoria.ApplyBulletImpulse.Start();
                    break;
                case Keys.Right:
                    Game.Player.Character.CancelRagdoll();
                    break;
                default:
                    break;
            }
        }
#else
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            
        }
#endif

        private void OnTick(object sender, EventArgs e)
        {
            Player player = Game.Player;
            Ped character = player.Character;

            if (!_euphoriaCooldownTimer.Completed)
            {
                if (!_cancelCheck)
                {
                    Vector3 posDiff = _euphoriaData.AttackedPed.Position - _posLastFrame;
                    posDiff.Abs();

                    if (posDiff.X < 0.025f && posDiff.Y < 0.025f && posDiff.Z < 0.025f)
                    {
                        _frameTime += Game.LastFrameTime * Game.TimeScale;
                    }
                    else
                    {
                        _frameTime = 0;
                    }

                    if (_frameTime > 1.0f)
                    {
                        _cancelledRagdoll = false;
                        _cancelCheck = true;
                    }
                }

                if (character.IsRagdoll && !_cancelledRagdoll)
                {
                    character.CancelRagdoll();
                    
                    _cancelledRagdoll = true;
                    ThrowNotification($"Cancelled Ragdoll - {Game.GameTime}");
                }

                _euphoriaCooldownTimer.Update(Game.LastFrameTime * Game.TimeScale);
                _posLastFrame = character.Position;
                character.ClearLastWeaponDamage();
                return;
            }
            _cancelledRagdoll = true;
            _cancelCheck = false;
            _frameTime = 0;

            Ped attackingPed = World.GetNearbyPeds(character, _debugConfig.PedSearchRadius,
                Array.Empty<Model>()).FirstOrDefault(p => character.HasBeenDamagedBy(p));

            if (attackingPed != null && player.CanPlayerEuphoria(_euphoriaConfig))
                RunEuphoriaLogic(character, attackingPed);

            _posLastFrame = character.Position;
            character.ClearLastWeaponDamage();
        }

        private void RunEuphoriaLogic(Ped attackedPed, Ped attackingPed)
        {
            int euphoriaTime = _euphoriaConfig.UseRandomEuphoriaActiveTime
                ? _rnd.Next(_euphoriaConfig.MinimumEuphoriaActiveTime, _euphoriaConfig.MaximumEuphoriaActiveTime + 1)
                : _euphoriaConfig.BaseEuphoriaActiveTime;

            float euphoriaCooldown = _euphoriaConfig.UseRandomEuphoriaCooldown
                ? _euphoriaConfig.MinimumEuphoriaCooldownTime + (float)_rnd.NextDouble() * (_euphoriaConfig.MaximumEuphoriaCooldownTime - _euphoriaConfig.MinimumEuphoriaCooldownTime)
                : _euphoriaConfig.BaseEuphoriaCooldown;

            _euphoriaData = new EuphoriaData(attackedPed, attackingPed, euphoriaTime, euphoriaCooldown);

            ThrowNotification($"Timer Started: {euphoriaCooldown + euphoriaTime}");
            _euphoriaCooldownTimer.Restart(euphoriaCooldown + euphoriaTime);
            euphoriaTime *= 1000;
            
            Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, euphoriaTime, euphoriaTime, 1, 1, 1, 0);

            InitializeShotConfig(in attackedPed);
            
            HandlePedBones(in attackedPed);

            InitializeShotHeadLookConfig(attackedPed, attackingPed);

            InitializePointGunConfig(in attackedPed, attackingPed);

            InitializeBulletImpulseConfig(in attackedPed, in attackingPed);

            attackedPed.Euphoria.Shot.Start();
            attackedPed.Euphoria.ShotNewBullet.Start();
            attackedPed.Euphoria.ShotSnap.Start();
            attackedPed.Euphoria.ApplyBulletImpulse.Start();
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
            if (euphoriaBone.HasValue)
                ped.Euphoria.ShotNewBullet.BodyPart = (int)euphoriaBone;
            else
                ped.Euphoria.Shot.ReachForWound = false;
        }

        private void InitializeBulletImpulseConfig(in Ped attackedPed, in Ped attackingPed)
        {
            Vector3 distanceNormalized = new Vector3(attackedPed.Position.X - attackingPed.Position.X,
                attackedPed.Position.Y - attackingPed.Position.Y, attackedPed.Position.Z - attackingPed.Position.Z).Normalized;

            ThrowNotification(distanceNormalized.ToString());

            attackedPed.Euphoria.ApplyBulletImpulse.HitPoint = attackingPed.LastWeaponImpactPosition;
            attackedPed.Euphoria.ApplyBulletImpulse.Impulse = distanceNormalized * 50;
        }

        private void InitializePointGunConfig(in Ped attackedPed, Ped attackingPed)
        {
            if (_euphoriaConfig.PointGunConfig.AimAtAttacker)
            {
                attackedPed.Euphoria.PointGun.UseTurnToTarget = _euphoriaConfig.PointGunConfig.TurnToAttacker;
                attackedPed.Euphoria.PointGun.RightHandTarget = attackingPed.Position;
                ThrowNotification(
                    $"Attacking Ped Pos: {attackingPed.Position} - Offset: {new Vector3(attackedPed.Position.X - attackingPed.Position.X, attackedPed.Position.Y - attackingPed.Position.Y, attackedPed.Position.Z - attackingPed.Position.Z)}");
                attackedPed.Euphoria.PointGun.Start();
            }
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