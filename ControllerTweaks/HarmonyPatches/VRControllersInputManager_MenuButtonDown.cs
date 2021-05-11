using ControllerTweaks.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(VRControllersInputManager))]
    [HarmonyPatch("MenuButtonDown", MethodType.Normal)]
    public class VRControllersInputManager_MenuButtonDown
    {
        private static readonly MethodInfo transpilerMethodInfo = SymbolExtensions.GetMethodInfo((IEnumerable<CodeInstruction> instructions) => Transpiler(instructions));
        internal static readonly MethodInfo baseMethodInfo = typeof(VRControllersInputManager).GetMethod("MenuButtonDown");
        internal static readonly HarmonyMethod transpilerMethod = new HarmonyMethod(transpilerMethodInfo);

        internal static readonly MethodInfo getCustomInput = SymbolExtensions.GetMethodInfo(() => GetCustomInput());
        internal static bool failedPatch = false;
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            failedPatch = false;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int index = -1;
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand?.ToString() == "MenuButtonOculusTouch" && codes[i+1].opcode == OpCodes.Call)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1 && PluginConfig.Instance.PauseRemapEnabled)
            {
                codes.RemoveAt(index);
                codes.RemoveAt(index);
                CodeInstruction newInstruction = new CodeInstruction(OpCodes.Call, getCustomInput);
                codes.Insert(index, newInstruction);
            }
            else if (index == -1)
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

        internal static bool GetCustomInput()
        {
            bool pressed = false;
            foreach (var button in PluginConfig.Instance.PauseButtons)
            {
                pressed = pressed || OVRInput.Get(button, OVRInput.Controller.Touch);
            }
            return pressed;
        }
    }
}
