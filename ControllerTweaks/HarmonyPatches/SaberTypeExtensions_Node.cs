using HarmonyLib;
using System.Reflection;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberTypeExtensions))]
    [HarmonyPatch("Node", MethodType.Normal)]
    public class SaberTypeExtensions_Node
    {
        private static readonly MethodInfo prefixMethodInfo = SymbolExtensions.GetMethodInfo((SaberType saberType) => Prefix(ref saberType));
        internal static readonly MethodInfo baseMethodInfo = SymbolExtensions.GetMethodInfo((SaberType saberType) => SaberTypeExtensions.Node(saberType));
        internal static readonly HarmonyMethod prefixMethod = new HarmonyMethod(prefixMethodInfo);
        internal static void Prefix(ref SaberType saberType)
        {
            if (saberType == SaberType.SaberA)
            {
                saberType = SaberType.SaberB;
            }
            else
            {
                saberType = SaberType.SaberA;
            }
        }
    }
}
