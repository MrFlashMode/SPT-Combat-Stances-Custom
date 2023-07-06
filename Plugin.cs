using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace CombatStances
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool IsAiming;
        public static bool IsBlindFiring;
        public static bool HasOptic;
        public static bool IsAllowedADS;
        public static bool RightArmBlacked;
        public static bool LeftArmBlacked;

        public static bool playerIsScav = false;

        public static bool IsSprinting;
        public static bool DidWeaponSwap;
        public static bool IsInInventory = false;

        public static float BaseWeaponLength;
        public static float NewWeaponLength;

        public static float RemainingArmStamPercentage = 1f;
        public static float ADSInjuryMulti;

        public static float BaseHipfireAccuracy;

        public static float AimSpeed;
        public static float WeaponSkillErgo = 0f;
        public static float AimSkillADSBuff = 0f;
        public static float AimMoveSpeedInjuryReduction;
        public static float TotalHandsIntensity = 1f;

        public static float Timer = 0.0f;
        public static int ShotCount = 0;
        public static int PrevShotCount = ShotCount;

        public static bool IsInThirdPerson = false;

        public static Vector3 TransformBaseStartPosition;
        public static Vector3 WeaponOffsetPosition;


        public static Player.BetterValueBlender StanceBlender = new Player.BetterValueBlender
        {
            Speed = 5f,
            Target = 0f
        };

        public static ConfigEntry<bool> EnableFSPatch { get; set; }
        public static ConfigEntry<bool> EnableNVGPatch { get; set; }

        public static ConfigEntry<bool> ToggleActiveAim { get; set; }
        public static ConfigEntry<bool> StanceToggleDevice { get; set; }

        public static ConfigEntry<bool> EnableAltPistol { get; set; }
        public static ConfigEntry<bool> EnableIdleStamDrain { get; set; }
        public static ConfigEntry<bool> EnableStanceStamChanges { get; set; }
        public static ConfigEntry<bool> EnableTacSprint { get; set; }

        // Delete some BepInEx parameetrs
        public static KeyboardShortcut CycleStancesKeybind = KeyboardShortcut.Empty;
        public static KeyboardShortcut ActiveAimKeybind = KeyboardShortcut.Empty;
        public static KeyboardShortcut HighReadyKeybind = KeyboardShortcut.Empty;
        public static KeyboardShortcut LowReadyKeybind = KeyboardShortcut.Empty;
        public static KeyboardShortcut ShortStockKeybind = KeyboardShortcut.Empty;

        public static float WeapOffsetX = 0.0f;
        public static float WeapOffsetY = 0.0f;
        public static float WeapOffsetZ = 0.0f;

        public static float StanceTransitionSpeed = 5.0f;
        public static float ThirdPersonRotationSpeed = 1.5f;
        public static float ThirdPersonPositionSpeed = 2.0f;

        public static float ActiveAimAdditionalRotationSpeedMulti = 1.0f;
        public static float ActiveAimResetRotationSpeedMulti = 3.0f;
        public static float ActiveAimRotationMulti = 1.0f;
        public static float ActiveAimSpeedMulti = 12.0f;
        public static float ActiveAimResetSpeedMulti = 9.6f;

        public static float ActiveAimOffsetX = -0.04f;
        public static float ActiveAimOffsetY = -0.01f;
        public static float ActiveAimOffsetZ = -0.01f;

        public static float ActiveAimRotationX = 0.0f;
        public static float ActiveAimRotationY = -30.0f;
        public static float ActiveAimRotationZ = 0.0f;

        public static float ActiveAimAdditionalRotationX = -1.5f;
        public static float ActiveAimAdditionalRotationY = -70f;
        public static float ActiveAimAdditionalRotationZ = 2f;

        public static float ActiveAimResetRotationX = 5.0f;
        public static float ActiveAimResetRotationY = 50.0f;
        public static float ActiveAimResetRotationZ = -3.0f;

        public static float HighReadyAdditionalRotationSpeedMulti = 1.25f;
        public static float HighReadyResetRotationMulti = 3.5f;
        public static float HighReadyRotationMulti = 1.8f;
        public static float HighReadyResetSpeedMulti = 6.0f;
        public static float HighReadySpeedMulti = 7.2f;

        public static float HighReadyOffsetX = 0.005f;
        public static float HighReadyOffsetY = 0.04f;
        public static float HighReadyOffsetZ = -0.05f;

        public static float HighReadyRotationX = -10.0f;
        public static float HighReadyRotationY = 3.0f;
        public static float HighReadyRotationZ = 3.0f;

        public static float HighReadyAdditionalRotationX = -10.0f;
        public static float HighReadyAdditionalRotationY = 10.0f;
        public static float HighReadyAdditionalRotationZ = 5f;

        public static float HighReadyResetRotationX = 0.5f;
        public static float HighReadyResetRotationY = 2f;
        public static float HighReadyResetRotationZ = 1.0f;

        public static float LowReadyAdditionalRotationSpeedMulti = 0.5f;
        public static float LowReadyResetRotationMulti = 2.5f;
        public static float LowReadyRotationMulti = 2.0f;
        public static float LowReadySpeedMulti = 18f;
        public static float LowReadyResetSpeedMulti = 7.2f;

        public static float LowReadyOffsetX = -0.01f;
        public static float LowReadyOffsetY = -0.01f;
        public static float LowReadyOffsetZ = 0.0f;

        public static float LowReadyRotationX = 8f;
        public static float LowReadyRotationY = -5.0f;
        public static float LowReadyRotationZ = -1.0f;

        public static float LowReadyAdditionalRotationX = 12.0f;
        public static float LowReadyAdditionalRotationY = -50.0f;
        public static float LowReadyAdditionalRotationZ = 0.5f;

        public static float LowReadyResetRotationX = -2.0f;
        public static float LowReadyResetRotationY = 2.0f;
        public static float LowReadyResetRotationZ = -0.5f;

        public static float PistolAdditionalRotationSpeedMulti = 1f;
        public static float PistolResetRotationSpeedMulti = 5f;
        public static float PistolRotationSpeedMulti = 1f;
        public static float PistolPosSpeedMulti = 12.0f;
        public static float PistolPosResetSpeedMulti = 12.0f;

        public static float PistolOffsetX = 0.015f;
        public static float PistolOffsetY = 0.04f;
        public static float PistolOffsetZ = -0.04f;

        public static float PistolRotationX = 0.0f;
        public static float PistolRotationY = -15f;
        public static float PistolRotationZ = 0.0f;

        public static float PistolAdditionalRotationX = 0f;
        public static float PistolAdditionalRotationY = -15.0f;
        public static float PistolAdditionalRotationZ = 0f;

        public static float PistolResetRotationX = 1.5f;
        public static float PistolResetRotationY = 2.0f;
        public static float PistolResetRotationZ = 1.2f;

        public static float ShortStockAdditionalRotationSpeedMulti = 2.0f;
        public static float ShortStockResetRotationSpeedMulti = 2.0f;
        public static float ShortStockRotationMulti = 2.0f;
        public static float ShortStockSpeedMulti = 6.0f;
        public static float ShortStockResetSpeedMulti = 6.0f;

        public static float ShortStockOffsetX = 0.02f;
        public static float ShortStockOffsetY = 0.1f;
        public static float ShortStockOffsetZ = -0.025f;

        public static float ShortStockRotationX = 0f;
        public static float ShortStockRotationY = -15.0f;
        public static float ShortStockRotationZ = 0.0f;

        public static float ShortStockAdditionalRotationX = -5.0f;
        public static float ShortStockAdditionalRotationY = -20.0f;
        public static float ShortStockAdditionalRotationZ = 5.0f;

        public static float ShortStockResetRotationX = -5.0f;
        public static float ShortStockResetRotationY = 12.0f;
        public static float ShortStockResetRotationZ = 1.0f;
        //

        private void Awake()
        {
            string miscSettings = "1. Misc. Settings";
            string weapAimAndPos = "2. Weapon Stances And Position";

            EnableNVGPatch = Config.Bind(miscSettings, "Enable NVG ADS Patch", true, new ConfigDescription("Magnified Optics Block ADS When Using NVGs.", null, new ConfigurationManagerAttributes { Order = 2 }));
            EnableFSPatch = Config.Bind(miscSettings, "Enable Faceshield Patch", true, new ConfigDescription("Faceshields Block ADS Unless The Specfic Stock/Weapon/Faceshield Allows It.", null, new ConfigurationManagerAttributes { Order = 1 }));

            EnableTacSprint = Config.Bind(weapAimAndPos, "Enable High Ready Sprint Animation", false, new ConfigDescription("Enables Usage Of High Ready Sprint Animation When Sprinting From High Ready Position.", null, new ConfigurationManagerAttributes { Order = 6 }));
            EnableAltPistol = Config.Bind(weapAimAndPos, "Enable Alternative Pistol Position And ADS", true, new ConfigDescription("Pistol Will Be Held Centered And In A Compressed Stance. ADS Will Be Animated.", null, new ConfigurationManagerAttributes { Order = 5 }));
            EnableIdleStamDrain = Config.Bind(weapAimAndPos, "Enable Idle Arm Stamina Drain", false, new ConfigDescription("Arm Stamina Will Drain When Not In A Stance (High And Low Ready, Short-Stocking).", null, new ConfigurationManagerAttributes { Order = 4 }));
            EnableStanceStamChanges = Config.Bind(weapAimAndPos, "Enable Stance Stamina And Movement Effects", true, new ConfigDescription("Enabled Stances To Affect Stamina And Movement Speed. High + Low Ready, Short-Stocking And Pistol Idle Will Regenerate Stamina Faster And Optionally Idle With Rifles Drains Stamina. High Ready Has Faster Sprint Speed And Sprint Accel, Low Ready Has Faster Sprint Accel. Arm Stamina Won't Drain Regular Stamina If It Reaches 0.", null, new ConfigurationManagerAttributes { Order = 3 }));
            ToggleActiveAim = Config.Bind(weapAimAndPos, "Use Toggle For Active Aim", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
            StanceToggleDevice = Config.Bind(weapAimAndPos, "Stance Toggles Off Light/Laser", true, new ConfigDescription("Entering High/Low Ready Will Toggle Off Lights/Lasers.", null, new ConfigurationManagerAttributes { Order = 1 }));

            new ApplyComplexRotationPatch().Enable();
            new ApplySimpleRotationPatch().Enable();
            new InitTransformsPatch().Enable();
            new WeaponOverlappingPatch().Enable();
            new WeaponLengthPatch().Enable();
            new OnWeaponDrawPatch().Enable();
            new WeaponOverlapViewPatch().Enable();
            new ZeroAdjustmentsPatch().Enable();
            new PlayerLateUpdatePatch().Enable();
            new SprintAccelerationPatch().Enable();
            new SetAimingSlowdownPatch().Enable();
            new RegisterShotPatch().Enable();
            new PlayerInitPatch().Enable();
            new SyncWithCharacterSkillsPatch().Enable();
            new method_20Patch().Enable();
            new UpdateHipInaccuracyPatch().Enable();
            new SetAimingPatch().Enable();
            new ToggleAimPatch().Enable();
            new SetFireModePatch().Enable();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            if (Utils.CheckIsReady())
            {

                if (ShotCount > PrevShotCount)
                {
                    StanceController.IsFiringFromStance = true;
                    PrevShotCount = ShotCount;
                }

                if (ShotCount == PrevShotCount)
                {
                    Timer += Time.deltaTime;
                    StanceController.StanceShotTimer();
                }

                if (Utils.WeaponReady == true)
                {
                    GameWorld gameWorld = Singleton<GameWorld>.Instance;
                    Player player = gameWorld.AllPlayers[0];
                    StanceController.StanceState(player.HandsController.Item as Weapon);
                }
            }
        }
    }
}
