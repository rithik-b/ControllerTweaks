using ControllerTweaks.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(VRControllersInputManager))]
    [HarmonyPatch("TriggerValue", MethodType.Normal)]
    public class VRControllersInputManager_TriggerValue
    {
        internal static readonly MethodInfo getLeftInput = SymbolExtensions.GetMethodInfo(() => GetLeftInput());
        internal static readonly MethodInfo getRightInput = SymbolExtensions.GetMethodInfo(() => GetRightInput());
        internal static bool failedPatch = false;
        internal static IVRPlatformHelper vrPlatformHelper = null;

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            if (vrPlatformHelper?.vrPlatformSDK == VRPlatformSDK.Oculus)
            {
                // Set remap to off if list is empty for safety reasons
                if (PluginConfig.Instance.LeftSelectButtons.Count == 0)
                {
                    PluginConfig.Instance.LeftSelectRemapEnabled = false;
                }

                if (PluginConfig.Instance.RightSelectButtons.Count == 0)
                {
                    PluginConfig.Instance.RightSelectRemapEnabled = false;
                }

                int firstIndex = -1;
                int secondIndex = -1;

                for (int i = 0; i < codes.Count - 1; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldstr && codes[i + 1].opcode == OpCodes.Call)
                    {
                        if (codes[i].operand?.ToString() == "TriggerLeftHand")
                        {
                            firstIndex = i;
                        }
                        else if (codes[i].operand?.ToString() == "TriggerRightHand")
                        {
                            secondIndex = i;
                        }
                    }
                }

                if (firstIndex != -1 && PluginConfig.Instance.LeftSelectRemapEnabled)
                {
                    codes.RemoveAt(firstIndex);
                    codes.RemoveAt(firstIndex);
                    CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getLeftInput);
                    codes.Insert(firstIndex, newInstruction);
                }

                if (secondIndex != -1 && PluginConfig.Instance.RightSelectRemapEnabled)
                {
                    codes.RemoveAt(secondIndex);
                    codes.RemoveAt(secondIndex);
                    CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getRightInput);
                    codes.Insert(secondIndex, newInstruction);
                }

                if (firstIndex == -1 || secondIndex == -1)
                {
                    failedPatch = true;
                }
            }

            return codes.AsEnumerable();
        }

        internal static float GetLeftInput()
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.LeftSelectButtons)
            {
                pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
            }
            return pressed ? 1 : 0;
        }

        internal static float GetRightInput()
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.RightSelectButtons)
            {
                pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
            }
            return pressed ? 1 : 0;
        }
    }
}