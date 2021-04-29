using ControllerTweaks.Configuration;
using HarmonyLib;
using UnityEngine.XR;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberTypeExtensions))]
    [HarmonyPatch("Node", MethodType.Normal)]
    public class SaberTypeExtensions_Node
    {
        internal static void Postfix(ref XRNode __result)
        {
            if (PluginConfig.Instance.ControllerSwapEnabled)
            {
                if (__result == XRNode.LeftHand)
                {
                    __result = XRNode.RightHand;
                }
                else
                {
                    __result = XRNode.LeftHand;
                }
            }
        }
    }
}
