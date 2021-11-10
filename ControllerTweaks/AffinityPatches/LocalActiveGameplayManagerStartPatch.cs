using SiraUtil.Affinity;
using System;

namespace ControllerTweaks.AffinityPatches
{
    internal class LocalActiveGameplayManagerStartPatch : IAffinity
    {
        public event Action<MultiplayerLocalActivePlayerGameplayManager> MultiplayerLocalActivePlayerGameplayManagerHasStarted;

        [AffinityPatch(typeof(MultiplayerLocalActivePlayerGameplayManager), nameof(MultiplayerLocalActivePlayerGameplayManager.Start))]
        private void Patch(MultiplayerLocalActivePlayerGameplayManager __instance)
        {
            MultiplayerLocalActivePlayerGameplayManagerHasStarted?.Invoke(__instance);
        }
    }
}
