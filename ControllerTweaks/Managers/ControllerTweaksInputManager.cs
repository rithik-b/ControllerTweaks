using ControllerTweaks.Configuration;
using ControllerTweaks.HarmonyPatches;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ControllerTweaks.Managers
{
    public class ControllerTweaksInputManager : IInitializable
    {
        private IVRPlatformHelper vrPlatformHelper;

        public static readonly Dictionary<string, OVRInput.Button> NameToButton = new Dictionary<string, OVRInput.Button>
        {
            { "Start", OVRInput.Button.Start },
            { "Left Stick", OVRInput.Button.PrimaryThumbstick },
            { "Right Stick", OVRInput.Button.SecondaryThumbstick },
            { "Left Trigger", OVRInput.Button.PrimaryIndexTrigger },
            { "Right Trigger", OVRInput.Button.SecondaryIndexTrigger },
            { "Left Grip", OVRInput.Button.PrimaryHandTrigger },
            { "Right Grip", OVRInput.Button.SecondaryHandTrigger },
            { "A", OVRInput.Button.One },
            { "B", OVRInput.Button.Two },
            { "X", OVRInput.Button.Three },
            { "Y", OVRInput.Button.Four }
        };

        public static Dictionary<OVRInput.Button, string> ButtonToName = new Dictionary<OVRInput.Button, string>
        {
            { OVRInput.Button.Start, "Start" },
            { OVRInput.Button.PrimaryThumbstick, "Left Stick" },
            { OVRInput.Button.SecondaryThumbstick, "Right Stick" },
            { OVRInput.Button.PrimaryIndexTrigger, "Left Trigger" },
            { OVRInput.Button.SecondaryIndexTrigger, "Right Trigger" },
            { OVRInput.Button.PrimaryHandTrigger, "Left Grip" },
            { OVRInput.Button.SecondaryHandTrigger, "Right Grip" },
            { OVRInput.Button.One, "A" },
            { OVRInput.Button.Two, "B" },
            { OVRInput.Button.Three, "X" },
            { OVRInput.Button.Four, "Y" }
        };

        public ControllerTweaksInputManager(IVRPlatformHelper vrPlatformHelper)
        {
            this.vrPlatformHelper = vrPlatformHelper;
        }

        public void Initialize()
        {
            UpdateInputPatches();
        }

        internal void UpdateInputPatches()
        {
            if (vrPlatformHelper.vrPlatformSDK == VRPlatformSDK.Oculus)
            {
                Plugin.harmony.Unpatch(VRControllersInputManager_MenuButtonDown.baseMethodInfo, HarmonyLib.HarmonyPatchType.Transpiler, Plugin.HarmonyId);
                Plugin.harmony.Unpatch(VRControllersInputManager_TriggerValue.baseMethodInfo, HarmonyLib.HarmonyPatchType.Transpiler, Plugin.HarmonyId);
                if (PluginConfig.Instance.PauseRemapEnabled && !Plugin.harmony.GetPatchedMethods().Contains(VRControllersInputManager_MenuButtonDown.baseMethodInfo))
                {
                    Plugin.harmony.Patch(VRControllersInputManager_MenuButtonDown.baseMethodInfo, transpiler: VRControllersInputManager_MenuButtonDown.transpilerMethod);
                }

                if ((PluginConfig.Instance.LeftSelectRemapEnabled || PluginConfig.Instance.RightSelectRemapEnabled) && !Plugin.harmony.GetPatchedMethods().Contains(VRControllersInputManager_TriggerValue.baseMethodInfo))
                {
                    Plugin.harmony.Patch(VRControllersInputManager_TriggerValue.baseMethodInfo, transpiler: VRControllersInputManager_TriggerValue.transpilerMethod);
                }
                else
                {
                }
            }
        }
    }
}
