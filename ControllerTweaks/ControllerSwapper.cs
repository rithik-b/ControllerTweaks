using ControllerTweaks.HarmonyPatches;
using IPA.Utilities;
using System;
using System.Linq;
using UnityEngine;
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
        }

        public void Dispose()
        {
            MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
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
