using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager))]
    [HarmonyPatch("Update", MethodType.Normal)]
    public class ObstacleSaberSparkleEffectManager_Update
    {
        // This Transpiler is needed because for some reason the base SaberTypeExtensions_Node is called without harmony patches if this is not done

        private static readonly MethodInfo transpilerMethodInfo = SymbolExtensions.GetMethodInfo((IEnumerable<CodeInstruction> instructions) => Transpiler(instructions));
        internal static readonly MethodBase baseMethodInfo = typeof(ObstacleSaberSparkleEffectManager).GetMethod("Update");
        internal static readonly HarmonyMethod transpilerMethod = new HarmonyMethod(transpilerMethodInfo);

        internal static readonly MethodInfo swapNode = SymbolExtensions.GetMethodInfo((SaberType saberType) => SwapNode(saberType));
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int index = -1;
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand?.ToString() == "UnityEngine.XR.XRNode Node(SaberType)")
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                codes[index] = new CodeInstruction(OpCodes.Call, swapNode);
            }
            return codes.AsEnumerable();
        }

        internal static UnityEngine.XR.XRNode SwapNode(SaberType saberType)
        {
            if (saberType == SaberType.SaberA)
            {
                return UnityEngine.XR.XRNode.RightHand;
            }
            return UnityEngine.XR.XRNode.LeftHand;
        }
    }
}
