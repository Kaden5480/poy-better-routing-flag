using HarmonyLib;
using UnityEngine;

namespace BetterRoutingFlag.Patches {
    [HarmonyPatch(typeof(FallingRock), "InitialiseRock")]
    static class PatchFallingRock {
        static bool Prefix() {
            return Flag.IsUsingFlag() == false;
        }
    }

    [HarmonyPatch(typeof(IceFallRandomiser), "SpawnRandomly")]
    static class PatchIceFall {
        static bool Prefix() {
            return Flag.IsUsingFlag() == false;
        }
    }
}

