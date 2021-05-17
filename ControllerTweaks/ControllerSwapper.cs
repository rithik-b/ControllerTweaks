using ControllerTweaks.HarmonyPatches;
using ControllerTweaks.Utilities;
using System;
using System.Linq;
using Zenject;

namespace ControllerTweaks
{
    public class ControllerSwapper : IInitializable, IDisposable
    {
        private VRController leftController;
        private VRController rightController;

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

            if (!Plugin.harmony.GetPatchedMethods().Contains(SaberTypeExtensions_Node.baseMethodInfo))
            {
                Plugin.harmony.Patch(SaberTypeExtensions_Node.baseMethodInfo, prefix: SaberTypeExtensions_Node.prefixMethod);
            }
            if (!Plugin.harmony.GetPatchedMethods().Contains(ObstacleSaberSparkleEffectManager_Update.baseMethodInfo))
            {
                Plugin.harmony.Patch(ObstacleSaberSparkleEffectManager_Update.baseMethodInfo, transpiler: ObstacleSaberSparkleEffectManager_Update.transpilerMethod);
            }
        }

        public void Dispose()
        {
            MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
        }

        private void MultiplayerLocalActivePlayerGameplayManagerHasStarted(MultiplayerLocalActivePlayerGameplayManager multiplayerLocalActivePlayerGameplayManager)
        {
            MultiplayerLocalActivePlayerGameplayManager_Start.MultiplayerLocalActivePlayerGameplayManagerHasStarted -= MultiplayerLocalActivePlayerGameplayManagerHasStarted;
            SaberManager saberManager = Accessors.SaberManagerAccessor(ref multiplayerLocalActivePlayerGameplayManager);
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
            leftController.node = UnityEngine.XR.XRNode.RightHand;
            rightController.node = UnityEngine.XR.XRNode.LeftHand;
        }
    }
}
