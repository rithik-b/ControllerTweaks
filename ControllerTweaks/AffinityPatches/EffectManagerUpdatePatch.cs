using HarmonyLib;
using SiraUtil.Affinity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ControllerTweaks.AffinityPatches
{
    internal class EffectManagerUpdatePatch : IAffinity
    {
        private static readonly MethodInfo swapNode = SymbolExtensions.GetMethodInfo((SaberType saberType) => SwapNode(saberType));

        [AffinityTranspiler]
        [AffinityPatch(typeof(ObstacleSaberSparkleEffectManager), nameof(ObstacleSaberSparkleEffectManager.Update))]
        private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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

        private static UnityEngine.XR.XRNode SwapNode(SaberType saberType)
        {
            if (saberType == SaberType.SaberA)
            {
                return UnityEngine.XR.XRNode.RightHand;
            }
            return UnityEngine.XR.XRNode.LeftHand;
        }
    }
}
