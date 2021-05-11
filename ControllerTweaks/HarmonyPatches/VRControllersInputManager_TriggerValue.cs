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
        private static readonly MethodInfo transpilerMethodInfo = SymbolExtensions.GetMethodInfo((IEnumerable<CodeInstruction> instructions) => Transpiler(instructions));
        internal static readonly MethodBase baseMethodInfo = typeof(VRControllersInputManager).GetMethod("TriggerValue");
        internal static readonly HarmonyMethod transpilerMethod = new HarmonyMethod(transpilerMethodInfo);

        internal static readonly MethodInfo getLeftInput = SymbolExtensions.GetMethodInfo(() => GetLeftInput());
        internal static readonly MethodInfo getRightInput = SymbolExtensions.GetMethodInfo(() => GetRightInput());
        internal static bool failedPatch = false;

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            failedPatch = false;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Set remap to off if list is empty for safety reasons
            if (PluginConfig.Instance.LeftSelectButtons.Count == 0)
            {
                PluginConfig.Instance.LeftSelectRemapEnabled = false;
            }

            if (PluginConfig.Instance.RightSelectButtons.Count == 0)
            {
                PluginConfig.Instance.RightSelectRemapEnabled = false;
            }

            int index = -1;

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i + 1].opcode == OpCodes.Call && codes[i].operand?.ToString() == "TriggerLeftHand")
                {
                    index = i;
                    break;
                }
            }

            if (index != -1 && PluginConfig.Instance.LeftSelectRemapEnabled)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getLeftInput);
                codes.Insert(index, newInstruction);
            }
            else if (index == -1)
            {
                failedPatch = true;
                Plugin.Log.Error("Select button remap patch failed.");
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    Plugin.Log.Error(codes[i].opcode + " " + codes[i].operand);
                }
                return codes.AsEnumerable();
            }

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i + 1].opcode == OpCodes.Call && codes[i].operand?.ToString() == "TriggerRightHand")
                {
                    index = i;
                    break;
                }
            }

            if (index != -1 && PluginConfig.Instance.RightSelectRemapEnabled)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getRightInput);
                codes.Insert(index, newInstruction);
            }
            else if (index == -1)
            {
                failedPatch = true;
                Plugin.Log.Error("Select button remap patch failed. Opcodes:");
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    Plugin.Log.Error(codes[i].opcode + " " + codes[i].operand);
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