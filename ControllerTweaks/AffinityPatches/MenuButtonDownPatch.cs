using ControllerTweaks.Configuration;
using SiraUtil.Affinity;

namespace ControllerTweaks.AffinityPatches
{
    internal class MenuButtonDownPatch : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(VRControllersInputManager), nameof(VRControllersInputManager.MenuButtonDown))]
        private bool Patch(ref bool __result)
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.PauseButtons)
            {
                pressed = pressed || OVRInput.GetDown(button, OVRInput.Controller.Touch);
            }
            __result = pressed;
            return false;
        }
    }
}
