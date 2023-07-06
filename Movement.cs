using System;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using HarmonyLib;
using UnityEngine;
using static EFT.Player;

namespace CombatStances
{
    public class SetAimingSlowdownPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GClass1603).GetMethod("SetAimingSlowdown", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool Prefix(ref GClass1603 __instance, bool isAiming, float slow)
        {
            Player player = (Player)AccessTools.Field(typeof(GClass1603), "player_0").GetValue(__instance);

            if (player.IsYourPlayer == true)
            {
                if (isAiming)
                {
                    //slow is hard set to 0.33 when called, 0.4-0.43 feels best.
                    float baseSpeed = slow + 0.07f - Plugin.AimMoveSpeedInjuryReduction;
                    float totalSpeed = StanceController.IsActiveAiming ? baseSpeed * 1.45f : baseSpeed;
                    __instance.AddStateSpeedLimit(Math.Max(totalSpeed, 0.15f), ESpeedLimit.Aiming);

                    return false;
                }

                __instance.RemoveStateSpeedLimit(ESpeedLimit.Aiming);
                return false;
            }
            return true;
        }
    }

    public class SprintAccelerationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GClass1603).GetMethod("SprintAcceleration", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool Prefix(GClass1603 __instance, float deltaTime)
        {
            Player player = (Player)AccessTools.Field(typeof(GClass1603), "player_0").GetValue(__instance);

            if (player.IsYourPlayer == true)
            {
                GClass755 rotationFrameSpan = (GClass755)AccessTools.Field(typeof(GClass1603), "gclass755_0").GetValue(__instance);
                float stanceAccelBonus = !Plugin.playerIsScav && Plugin.IsSprinting && Plugin.EnableTacSprint.Value && !Plugin.LeftArmBlacked && !Plugin.RightArmBlacked ? 2f : 1f;
                float stanceSpeedBonus = !Plugin.playerIsScav && Plugin.IsSprinting && Plugin.EnableTacSprint.Value && !Plugin.LeftArmBlacked && !Plugin.RightArmBlacked ? 2f : 1f;

                float sprintAccel = player.Physical.SprintAcceleration * deltaTime * stanceAccelBonus;
                float speed = (player.Physical.SprintSpeed * __instance.SprintingSpeed + 1f) * __instance.StateSprintSpeedLimit * stanceSpeedBonus;
                float sprintInertia = Mathf.Max(EFTHardSettings.Instance.sprintSpeedInertiaCurve.Evaluate(Mathf.Abs((float)rotationFrameSpan.Average)), EFTHardSettings.Instance.sprintSpeedInertiaCurve.Evaluate(2.1474836E+09f) * (2f - player.Physical.Inertia));
                speed = Mathf.Clamp(speed * sprintInertia, 0.1f, speed);
                __instance.SprintSpeed = Mathf.Clamp(__instance.SprintSpeed + sprintAccel * Mathf.Sign(speed - __instance.SprintSpeed), 0.01f, speed);

                return false;
            }
            return true;
        }
    }

    public class PlayerLateUpdatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            if (Utils.CheckIsReady() == true && __instance.IsYourPlayer == true)
            {
                FirearmController fc = __instance.HandsController as FirearmController;
                PlayerInjuryStateCheck(__instance);
                Plugin.IsSprinting = __instance.IsSprintEnabled;
                Plugin.IsInInventory = __instance.IsInventoryOpened;

                if (fc != null)
                {
                    AimController.ADSCheck(__instance, fc);

                    if (Plugin.EnableStanceStamChanges.Value == true)
                    {
                        StanceController.SetStanceStamina(__instance, fc);
                    }

                    Plugin.RemainingArmStamPercentage = Mathf.Min(__instance.Physical.HandsStamina.Current * 1.65f, __instance.Physical.HandsStamina.TotalCapacity) / __instance.Physical.HandsStamina.TotalCapacity;
                }
                else if (Plugin.EnableStanceStamChanges.Value == true)
                {
                    StanceController.ResetStanceStamina(__instance);
                }

                __instance.Physical.HandsStamina.Current = Mathf.Max(__instance.Physical.HandsStamina.Current, 1f);
            }
        }

        public static void PlayerInjuryStateCheck(Player player)
        {
            bool rightArmDamaged = player.MovementContext.PhysicalConditionIs(EPhysicalCondition.RightArmDamaged);
            bool leftArmDamaged = player.MovementContext.PhysicalConditionIs(EPhysicalCondition.LeftArmDamaged);
            bool tremor = player.MovementContext.PhysicalConditionIs(EPhysicalCondition.Tremor);

            Plugin.RightArmBlacked = rightArmDamaged;
            Plugin.LeftArmBlacked = leftArmDamaged;

            if (!rightArmDamaged && !leftArmDamaged && !tremor)
            {
                Plugin.AimMoveSpeedInjuryReduction = 0f;
                Plugin.ADSInjuryMulti = 1f;
            }
            if (tremor == true)
            {
                Plugin.AimMoveSpeedInjuryReduction = 0.025f;
                Plugin.ADSInjuryMulti = 0.85f;
            }
            if ((rightArmDamaged == true && !leftArmDamaged))
            {
                Plugin.AimMoveSpeedInjuryReduction = 0.07f;
                Plugin.ADSInjuryMulti = 0.6f;
            }
            if ((!rightArmDamaged && leftArmDamaged == true))
            {
                Plugin.AimMoveSpeedInjuryReduction = 0.05f;
                Plugin.ADSInjuryMulti = 0.7f;
            }
            if (rightArmDamaged == true && leftArmDamaged == true)
            {
                Plugin.AimMoveSpeedInjuryReduction = 0.1f;
                Plugin.ADSInjuryMulti = 0.5f;
            }
        }
    }
}
