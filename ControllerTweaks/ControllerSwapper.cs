using ControllerTweaks.AffinityPatches;
using ControllerTweaks.Utilities;
using System;
using Zenject;

namespace ControllerTweaks
{
    internal class ControllerSwapper : IInitializable, IDisposable
    {
        private LocalActiveGameplayManagerStartPatch patch;
        private VRController leftController;
        private VRController rightController;

        public ControllerSwapper(LocalActiveGameplayManagerStartPatch patch, [InjectOptional] SaberManager saberManager)
        {
            this.patch = patch;
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
        }

        public void Initialize()
        {
            // If we are in multi we do a little harmony
            if (leftController == null || rightController == null)
            {
                patch.MultiplayerLocalActivePlayerGameplayManagerHasStarted += MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            }
            else
            {
                leftController.node = UnityEngine.XR.XRNode.RightHand;
                rightController.node = UnityEngine.XR.XRNode.LeftHand;
            }
        }

        public void Dispose()
        {
            patch.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
        }

        private void MultiplayerLocalActivePlayerGameplayManagerHasStarted(MultiplayerLocalActivePlayerGameplayManager multiplayerLocalActivePlayerGameplayManager)
        {
            patch.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            SaberManager saberManager = Accessors.SaberManagerAccessor(ref multiplayerLocalActivePlayerGameplayManager);
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
            leftController.node = UnityEngine.XR.XRNode.RightHand;
            rightController.node = UnityEngine.XR.XRNode.LeftHand;
        }
    }
}
