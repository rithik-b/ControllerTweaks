using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTweaks.HarmonyPatches
{
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPatch("PlayHapticFeedback", MethodType.Normal)]
    public class HapticFeedbackController_PlayHapticFeedback
    {
        internal static void Prefix()
        {
            StackTrace stackTrace = new StackTrace();
            foreach(var frame in stackTrace.GetFrames())
            {
            }
        }
    }
}
