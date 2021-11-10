using ControllerTweaks.Configuration;
using HarmonyLib;
using SiraUtil.Affinity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.AffinityPatches
{
    internal class MenuButtonDownPatch : IAffinity
    {
        private static readonly MethodInfo getCustomInput = SymbolExtensions.GetMethodInfo(() => GetCustomInput());
        public bool failedPatch = false;

        [AffinityTranspiler]
        [AffinityPatch(typeof(VRControllersInputManager), nameof(VRControllersInputManager.MenuButtonDown))]
        private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            failedPatch = false;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int index = -1;
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand?.ToString() == "MenuButtonOculusTouch" && codes[i + 1].opcode == OpCodes.Call)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getCustomInput);
                codes.Insert(index, newInstruction);
            }
            else
            {
                failedPatch = true;
                Plugin.Log.Error("Pause button remap patch failed. Opcodes:");
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    Plugin.Log.Error(codes[i].opcode + " " + codes[i].operand);
                }
            }
            return codes.AsEnumerable();
        }

        private static bool GetCustomInput()
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.PauseButtons)
            {
                pressed = pressed || OVRInput.GetDown(button, OVRInput.Controller.Touch);
            }
            return pressed;
        }
    }
}
