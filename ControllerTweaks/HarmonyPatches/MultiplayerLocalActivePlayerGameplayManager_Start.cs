using HarmonyLib;
using System;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLocalActivePlayerGameplayManager))]
    [HarmonyPatch("Start", MethodType.Normal)]
    public class MultiplayerLocalActivePlayerGameplayManager_Start
    {
        internal static event Action<MultiplayerLocalActivePlayerGameplayManager> MultiplayerLocalActivePlayerGameplayManagerHasStarted;
        internal static void Prefix(MultiplayerLocalActivePlayerGameplayManager __instance)
        {
            MultiplayerLocalActivePlayerGameplayManagerHasStarted?.Invoke(__instance);
        }
    }
}
