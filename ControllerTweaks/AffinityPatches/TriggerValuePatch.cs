using ControllerTweaks.Configuration;
using HarmonyLib;
using SiraUtil.Affinity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.AffinityPatches
{
    internal class TriggerValuePatch
    {
        private static readonly MethodInfo getLeftInput = SymbolExtensions.GetMethodInfo(() => GetLeftInput());
        private static readonly MethodInfo getRightInput = SymbolExtensions.GetMethodInfo(() => GetRightInput());
        public bool failedPatch = false;

        [AffinityTranspiler]
        [AffinityPatch(typeof(VRControllersInputManager), nameof(VRControllersInputManager.TriggerValue))]
        private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            failedPatch = false;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int index = -1;

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i + 1].opcode == OpCodes.Call && codes[i].operand?.ToString() == "TriggerLeftHand")
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getLeftInput);
                codes.Insert(index, newInstruction);
            }
            else
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

            if (index != -1)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getRightInput);
                codes.Insert(index, newInstruction);
            }
            else
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

        private static float GetLeftInput()
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.LeftSelectButtons)
            {
                pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
            }
            return pressed ? 1 : 0;
        }

        private static float GetRightInput()
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