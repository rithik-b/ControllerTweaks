using ControllerTweaks.Configuration;
using SiraUtil.Affinity;
using UnityEngine.XR;

namespace ControllerTweaks.AffinityPatches
{
    internal class TriggerValuePatch : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(VRControllersInputManager), nameof(VRControllersInputManager.TriggerValue))]
        private bool Patch(XRNode node, ref float __result)
        {
            if (node == XRNode.LeftHand && PluginConfig.Instance.LeftSelectRemapEnabled)
            {
                bool pressed = false;
                foreach (var button in PluginConfig.Instance.LeftSelectButtons)
                {
                    pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
                }
                __result = pressed ? 1 : 0;
            }
            else if (node == XRNode.RightHand && PluginConfig.Instance.RightSelectRemapEnabled)
            {
                bool pressed = false;
                foreach (var button in PluginConfig.Instance.RightSelectButtons)
                {
                    pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
                }
                __result = pressed ? 1 : 0;
            }
            else
            {
                return true; // Let the base method run as is
            }
            return false;
        }
    }
}