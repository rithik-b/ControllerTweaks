using HarmonyLib;
using System;
using System.Reflection;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLocalActivePlayerGameplayManager))]
    [HarmonyPatch("Start", MethodType.Normal)]
    public class MultiplayerLocalActivePlayerGameplayManager_Start
    {
        private static readonly MethodInfo prefixMethodInfo = SymbolExtensions.GetMethodInfo((MultiplayerLocalActivePlayerGameplayManager instance) => Prefix(instance));
        internal static readonly MethodInfo baseMethodInfo = SymbolExtensions.GetMethodInfo((MultiplayerLocalActivePlayerGameplayManager instance) => instance.Start());
        internal static readonly HarmonyMethod prefixMethod = new HarmonyMethod(prefixMethodInfo);

        internal static event Action<MultiplayerLocalActivePlayerGameplayManager> MultiplayerLocalActivePlayerGameplayManagerHasStarted;
        internal static void Prefix(MultiplayerLocalActivePlayerGameplayManager __instance)
        {
            MultiplayerLocalActivePlayerGameplayManagerHasStarted?.Invoke(__instance);
        }
    }
}
