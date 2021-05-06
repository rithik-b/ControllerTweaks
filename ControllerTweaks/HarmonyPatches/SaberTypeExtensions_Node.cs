using HarmonyLib;
using System.Reflection;
using UnityEngine.XR;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberTypeExtensions))]
    [HarmonyPatch("Node", MethodType.Normal)]
    public class SaberTypeExtensions_Node
    {
        private static readonly MethodInfo postfixMethodInfo = SymbolExtensions.GetMethodInfo((XRNode result) => Postfix(ref result));
        internal static readonly MethodInfo baseMethodInfo = SymbolExtensions.GetMethodInfo((SaberType saberType) => SaberTypeExtensions.Node(saberType));
        internal static readonly HarmonyMethod postfixMethod = new HarmonyMethod(postfixMethodInfo);
        internal static void Postfix(ref XRNode __result)
        {
            Plugin.Log.Critical("Patched");
            if (__result == XRNode.RightHand)
            {
                __result = XRNode.LeftHand;
            }
            else
            {
                __result = XRNode.RightHand;
            }
        }
    }
}
