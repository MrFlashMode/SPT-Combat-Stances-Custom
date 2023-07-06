using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using EFT.Animations;
using HarmonyLib;
using static EFT.Player;

namespace CombatStances
{
    public class method_20Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ProceduralWeaponAnimation).GetMethod("method_20", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(ref ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)AccessTools.Field(typeof(ProceduralWeaponAnimation), "firearmController_0").GetValue(__instance);

            if (firearmController != null)
            {
                Player player = (Player)AccessTools.Field(typeof(FirearmController), "_player").GetValue(firearmController);
                if (player.IsYourPlayer == true)
                {
                    Plugin.HasOptic = __instance.CurrentScope.IsOptic ? true : false;
                    Plugin.AimSpeed = (float)AccessTools.Field(typeof(ProceduralWeaponAnimation), "float_9").GetValue(__instance);
                    Plugin.TotalHandsIntensity = __instance.HandsContainer.HandsRotation.InputIntensity;
                }
            }
        }
    }

    public class SyncWithCharacterSkillsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmController).GetMethod("SyncWithCharacterSkills", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(ref FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                SkillsClass.GClass1680 skillsClass = (SkillsClass.GClass1680)AccessTools.Field(typeof(FirearmController), "gclass1680_0").GetValue(__instance);
                Plugin.WeaponSkillErgo = skillsClass.DeltaErgonomics;
                Plugin.AimSkillADSBuff = skillsClass.AimSpeed;
            }
        }
    }

    public class RegisterShotPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmController).GetMethod("RegisterShot", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                Plugin.Timer = 0f;
                StanceController.StanceShotTime = 0f;
                StanceController.IsFiringFromStance = true;
                Plugin.ShotCount++;
            }
        }
    }

    public class PlayerInitPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            if (__instance.IsYourPlayer == true)
            {
                Plugin.playerIsScav = __instance.Fraction == ETagStatus.Scav;
            }
        }
    }
}
