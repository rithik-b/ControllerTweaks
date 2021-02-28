using ControllerTweaks.Configuration;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ControllerTweaks
{
    class ControllerSwapper : IInitializable
    {
        private VRController leftController;
        private VRController rightController;

        ControllerSwapper(SaberManager saberManager)
        {
            leftController = saberManager.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager.rightSaber.GetComponentInParent<VRController>();
        }

        public void Initialize()
        {
            if (PluginConfig.Instance.ControllerSwapEnabled)
            {
                leftController.node = UnityEngine.XR.XRNode.RightHand;
                rightController.node = UnityEngine.XR.XRNode.LeftHand;
            }
        }
    }
}
