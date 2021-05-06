using ControllerTweaks.HarmonyPatches;
using IPA.Utilities;
using System;
using Zenject;

namespace ControllerTweaks
{
    class ControllerSwapper : IInitializable, IDisposable
    {
        private VRController leftController;
        private VRController rightController;

        public static readonly FieldAccessor<MultiplayerLocalActivePlayerGameplayManager, SaberManager>.Accessor SaberManagerAccessor = FieldAccessor<MultiplayerLocalActivePlayerGameplayManager, SaberManager>.GetAccessor("_saberManager");

        public ControllerSwapper([InjectOptional] SaberManager saberManager)
        {
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
        }

        public void Initialize()
        {
            // If we are in multi we do a little harmony
            if (leftController == null || rightController == null)
            {
                MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted += MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            }
            else
            {
                leftController.node = UnityEngine.XR.XRNode.RightHand;
                rightController.node = UnityEngine.XR.XRNode.LeftHand;
            }
            Plugin.harmony.Unpatch(SaberTypeExtensions_Node.baseMethodInfo, HarmonyLib.HarmonyPatchType.Postfix, Plugin.HarmonyId);
            Plugin.harmony.Patch(SaberTypeExtensions_Node.baseMethodInfo, postfix: SaberTypeExtensions_Node.postfixMethod);
        }

        public void Dispose()
        {
            MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            Plugin.harmony.Unpatch(SaberTypeExtensions_Node.baseMethodInfo, HarmonyLib.HarmonyPatchType.Postfix, Plugin.HarmonyId);
        }

        private void MultiplayerLocalActivePlayerGameplayManagerHasStarted(MultiplayerLocalActivePlayerGameplayManager multiplayerLocalActivePlayerGameplayManager)
        {
            MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            SaberManager saberManager = SaberManagerAccessor(ref multiplayerLocalActivePlayerGameplayManager);
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
            leftController.node = UnityEngine.XR.XRNode.RightHand;
            rightController.node = UnityEngine.XR.XRNode.LeftHand;
        }
    }
}
