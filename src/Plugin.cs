using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX

using BepInEx;

namespace BetterRoutingFlag {
    [BepInPlugin("com.github.Kaden5480.poy-better-routing-flag", "BetterRoutingFlag", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        public void Awake() {
            foreach (string sceneName in validScenes) {
                Rotation rotation = new Rotation(
                    Config.Bind(sceneName, "rotY", 0f),
                    Config.Bind(sceneName, "rotW", 0f),
                    Config.Bind(sceneName, "rotationY", 0f)
                );

                rotations.Add(sceneName, rotation);
            }

            Harmony.CreateAndPatchAll(typeof(Plugin.PatchRoutingFlagRestore));
            Harmony.CreateAndPatchAll(typeof(Plugin.PatchRoutingFlagSave));

            // Remove falling rocks
            Harmony.CreateAndPatchAll(typeof(Patches.PatchFallingRock));
            Harmony.CreateAndPatchAll(typeof(Patches.PatchIceFall));
        }

#elif MELONLOADER

using MelonLoader;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(BetterRoutingFlag.Plugin), "BetterRoutingFlag", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace BetterRoutingFlag {
    public class Plugin: MelonMod {
        public override void OnInitializeMelon() {
            foreach (string sceneName in validScenes) {
                MelonPreferences_Category scene = MelonPreferences.CreateCategory($"BetterRoutingFlag_{sceneName}");
                scene.SetFilePath(
                    $"{MelonEnvironment.UserDataDirectory}/com.github.Kaden5480.poy-better-routing-flag.cfg"
                );

                Rotation rotation = new Rotation(
                    scene.CreateEntry("rotY", 0f),
                    scene.CreateEntry("rotW", 0f),
                    scene.CreateEntry("rotationY", 0f)
                );

                rotations.Add(sceneName, rotation);
            }
        }

#endif
        private string[] validScenes = new string[] {
            "Peak_1_GreenhornNEW", "Peak_2_PaltryNEW", "Peak_3_OldMill", "Peak_3_GrayGullyNEW",
            "Peak_LighthouseNew", "Peak_4_OldManOfSjorNEW", "Peak_5_GiantsShelfNEW", "Peak_8_EvergreensEndNEW",
            "Peak_9_TheTwinsNEW", "Peak_6_OldGroveSkelf", "Peak_7_HangmansLeapNEW", "Peak_13_LandsEndNEW",
            "Peak_19_OldLangr", "Peak_14_Cavern", "Peak_16_ThreeSeaStacks", "Peak_10_WaltersCragNEW",
            "Peak_15_TheGreatCrevice", "Peak_17_RainyPeak", "Peak_18_FallingBoulders", "Peak_11_WutheringCrestNEW",

            "Boulder_1_OldWalkersBoulder", "Boulder_2_JotunnsThumb", "Boulder_3_OldSkerry", "Boulder_4_TheHamarrStone",
            "Boulder_5_GiantsNose", "Boulder_6_WaltersBoulder", "Boulder_7_SunderedSons", "Boulder_8_OldWealdsBoulder",
            "Boulder_9_LeaningSpire", "Boulder_10_Cromlech",

            "Tind_1_WalkersPillar", "Tind_3_GreatGaol", "Tind_2_Eldenhorn",
            "Tind_4_StHaelga", "Tind_5_YmirsShadow",

            "Category4_1_FrozenWaterfall", "Category4_2_SolemnTempest",

            "Alps_1_TrainingTower", "Alps_2_BalancingBoulder", "Alps_3_SeaArch",
            "Alps_4_SunfullSpire", "Alps_5_Tree", "Alps_6_Treppenwald",
            "Alps_7_Castle", "Alps_8_SeaSideTraining", "Alps_9_IvoryGranites",
            "Alps_10_Rekkja", "Alps_11_Quietude", "Alps_12_Overlook",

            "Alps2_1_Waterfall", "Alps2_2_Dam",
            "Alps2_3_Dunderhorn", "Alps2_4_ElfenbenSpires",
            "Alps2_5_WelkinPass",

            "Alps3_1_SeigrCraeg", "Alps3_2_UllrsGate",
            "Alps3_3_GreatSilf", "Alps3_4_ToweringVisir",
            "Alps3_5_EldrisWall", "Alps3_6_MountMhorgorm",
        };

        private static Dictionary<string, Rotation> rotations = new Dictionary<string, Rotation>();

        /**
         * <summary>
         * Gets a private instance field from the routing flag.
         * </summary>
         * <param name="flag">An instance of the routing flag</param>
         * <param name="fieldName">The name of the field to get</param>
         */
        private static T GetField<T>(RoutingFlag flag, string fieldName) {
            return (T) typeof(RoutingFlag).GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(flag);
        }

        /**
         * <summary>
         * Gets the rotation configuration for a specified scene.
         * </summary>
         * <param name="sceneName">The name of the scene to get the rotation for</param>
         * <return>A valid reference if found, null otherwise</return>
         */
        private static Rotation GetRotation(string sceneName) {
            Rotation rotation = null;

            if (rotations.TryGetValue(sceneName, out rotation) == false) {
                return null;
            }

            return rotation;
        }

        /**
         * <summary>
         * Resets crampons.
         * </summary>
         */
        private static void ResetCrampons() {
            StemFoot stemFoot = GameObject.Find("CramponsWallkick").GetComponent<StemFoot>();
            stemFoot.wallkickCooldown = 0f;
        }

        /**
         * <summary>
         * Resets stamina.
         * </summary>
         */
        private static void ResetStamina() {
            ClimbingPitches pitches = GameObject.Find("ClimbingPitches").GetComponent<ClimbingPitches>();
            MicroHolds microHolds = GameObject.Find("Player").GetComponent<MicroHolds>();
            IceAxe iceAxes = GameObject.Find("IceAxes").GetComponent<IceAxe>();

            Vector3 originalStaminaCircleScale = (Vector3) typeof(ClimbingPitches).GetField(
                "originalStaminaCircleScale",
                BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(pitches);

            // Crimps/pinches
            microHolds.leftHandGripStrength = 100f;
            microHolds.rightHandGripStrength = 100f;
            microHolds.leftHandGripStrength_Pinch = 100f;
            microHolds.rightHandGripStrength_Pinch = 100f;

            // Pitches
            pitches.staminaCircle.localScale = originalStaminaCircleScale;

            // Pickaxes
            iceAxes.iceAxeStaminaL = 100f;
            iceAxes.iceAxeStaminaR = 100f;
        }

        /**
         * <summary>
         * Patches the routing flag to save camera rotations.
         * </summary>
         */
        [HarmonyPatch(typeof(RoutingFlag), "SetRoutingFlagPosition")]
        static class PatchRoutingFlagSave {
            static void Postfix(RoutingFlag __instance) {
                CameraLook cameraLook = null;
                Transform playerCameraHolder = null;
                Scene scene = SceneManager.GetActiveScene();
                Rotation rotation = GetRotation(scene.name);

                if (rotation == null) {
                    return;
                }

                cameraLook = GameObject.Find("CamY").GetComponent<CameraLook>();
                playerCameraHolder = GameObject.Find("PlayerCameraHolder").transform;

                rotation.rotY = playerCameraHolder.rotation.y;
                rotation.rotW = playerCameraHolder.rotation.w;
                rotation.rotationY = cameraLook.rotationY;
            }
        }

        /**
         * <summary>
         * Patches the routing flag's Update method to restore camera rotations.
         * </summary>
         */
        [HarmonyPatch(typeof(RoutingFlag), "Update")]
        static class PatchRoutingFlagRestore {
            static void Prefix(RoutingFlag __instance) {
                CameraLook cameraLook = null;
                Transform playerCameraHolder = null;
                Scene scene;
                Rotation rotation = null;

                // Cases routing flag can't be used
                if (
                    __instance.currentlyUsingFlag == false
                    || InGameMenu.isCurrentlyNavigationMenu == true
                    || EnterPeakScene.enteringPeakScene == true
                    || ResetPosition.resettingPosition == true
                ) {
                    return;
                }

                // Checks specific to teleporting
                if (
                    GetField<Player>(__instance, "player").GetButtonDown("Move To Routing Flag") == false
                    || GetField<RopeAnchor>(__instance, "ropeanchor").attached == true
                    || Crampons.cramponsActivated == true
                    || Bivouac.currentlyUsingBivouac == true
                    || __instance.usedFlagTeleport == true
                    || __instance.flagPositionOnPeak_X[__instance.currentPeak] == 0
                    || __instance.flagPositionOnPeak_Y[__instance.currentPeak] == 0
                    || __instance.flagPositionOnPeak_Z[__instance.currentPeak] == 0
                ) {
                    return;
                }

                // Otherwise, update rotation
                scene = SceneManager.GetActiveScene();
                rotation = GetRotation(scene.name);

                if (rotation == null) {
                    return;
                }

                ResetCrampons();
                ResetStamina();

                cameraLook = GameObject.Find("CamY").GetComponent<CameraLook>();
                playerCameraHolder = GameObject.Find("PlayerCameraHolder").transform;

                playerCameraHolder.rotation = new Quaternion(0f, rotation.rotY, 0f, rotation.rotW);
                cameraLook.rotationY = rotation.rotationY;
            }
        }
    }
}
