using System;
using Euphorically.Config;
using GTA;
using GTA.Native;
using GTA.UI;

namespace Euphorically
{
    public class Main : Script
    {
        private readonly EuphoriaConfig _euphoriaConfig;
        private readonly DebugConfig _debugConfig;
        private readonly Random _rng = new Random();

        private bool _putInEuphoria;
        private float _timer;
        private float _timerGoal;

        public Main()
        {
            _euphoriaConfig = new EuphoriaConfig(Settings);
            _debugConfig = new DebugConfig(Settings);

            Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_putInEuphoria)
            {
                _timer += Game.LastFrameTime;
                if (_timer >= _timerGoal)
                {
                    _putInEuphoria = false;
                    _timer = 0;
                }

                Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                return;
            }

            if (Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2))
            {
                if (!_euphoriaConfig.EuphoriaFromMeleeDamage &&
                    Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 1))
                {
                    ThrowNotification("Caught Melee Damage?");
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    return;
                }

                if (!_euphoriaConfig.EuphoriaFromWeaponDamage &&
                    (Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 2) &&
                     !Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON, Game.Player.Character, 0, 1)))
                {
                    ThrowNotification("Caught Weapon Damage?");
                    Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
                    return;
                }

                int euphoriaTime = _euphoriaConfig.RandomizeEuphoriaTimeRange
                    ? _rng.Next(_euphoriaConfig.EuphoriaMinimumTime * 1000, _euphoriaConfig.EuphoriaMaximumTime * 1000)
                    : _euphoriaConfig.EuphoriaMinimumTime;

                ThrowNotification("Euphoria Time: " + euphoriaTime + "ms");

                EuphoriaType euphoriaType = _euphoriaConfig.EuphoriaType;

                if ((_euphoriaConfig.NormalEuphoriaInRandomize || _euphoriaConfig.StiffFallEuphoriaInRandomize ||
                     _euphoriaConfig.NarrowStumbleEuphoriaInRandomize ||
                     _euphoriaConfig.WideStumbleEuphoriaInRandomize) && _euphoriaConfig.RandomizeEuphoriaTypes)
                {
                    bool foundValidEuphoriaType = false;
                    while (!foundValidEuphoriaType)
                    {
                        EuphoriaType type = (EuphoriaType) _rng.Next(0, 4);
                        switch (type)
                        {
                            case EuphoriaType.Normal:
                                if (_euphoriaConfig.NormalEuphoriaInRandomize)
                                {
                                    foundValidEuphoriaType = true;
                                    euphoriaType = type;
                                }
                                break;
                            case EuphoriaType.StiffFall:
                                if (_euphoriaConfig.StiffFallEuphoriaInRandomize)
                                {
                                    foundValidEuphoriaType = true;
                                    euphoriaType = type;
                                }
                                break;
                            case EuphoriaType.NarrowStumble:
                                if (_euphoriaConfig.NarrowStumbleEuphoriaInRandomize)
                                {
                                    foundValidEuphoriaType = true;
                                    euphoriaType = type;
                                }
                                break;
                            case EuphoriaType.WideStumble:
                                if (_euphoriaConfig.WideStumbleEuphoriaInRandomize)
                                {
                                    foundValidEuphoriaType = true;
                                    euphoriaType = type;
                                }
                                break;
                            default:
                                return;
                        }
                    }
                }

                ThrowNotification("Euphoria Type: " + euphoriaType);

                int chanceValueRequirement = _euphoriaConfig.RandomizeEuphoriaChance
                    ? _rng.Next(_euphoriaConfig.EuphoriaMinimumChance, _euphoriaConfig.EuphoriaMaximumChance)
                    : _euphoriaConfig.EuphoriaMinimumChance;

                ThrowNotification("Chance Value Requirement: " + chanceValueRequirement);

                if (_rng.Next(0, 100) < chanceValueRequirement)
                {
                    RagdollWrapper(Game.Player.Character, euphoriaTime, euphoriaType);
                    _putInEuphoria = true;
                    _timerGoal = euphoriaTime / 1000f + _euphoriaConfig.EuphoriaAntiSpamTime;
                    ThrowNotification("Euphoria ran?");
                }
            }

            Function.Call(Hash.CLEAR_ENTITY_LAST_WEAPON_DAMAGE, Game.Player.Character);
        }

        private static void RagdollWrapper(Ped ped, int timeInMilliseconds, EuphoriaType euphoriaType)
        {
            Function.Call(Hash.SET_PED_CAN_RAGDOLL, ped, true);
            
            Function.Call(Hash.SET_PED_TO_RAGDOLL, ped, timeInMilliseconds, timeInMilliseconds, (int) euphoriaType,
                false, false, false);
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
