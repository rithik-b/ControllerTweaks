using ControllerTweaks.Configuration;
using ControllerTweaks.HarmonyPatches;
using ControllerTweaks.UI;
using System.Linq;
using Zenject;

namespace ControllerTweaks.Installers
{
    public class ControllerTweaksGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (PluginConfig.Instance.ControllerSwapEnabled)
            {
                Container.BindInterfacesTo<ControllerSwapper>().AsSingle();
            }
            else
            {
                if (Plugin.harmony.GetPatchedMethods().Contains(SaberTypeExtensions_Node.baseMethodInfo))
                {
                    Plugin.harmony.Unpatch(SaberTypeExtensions_Node.baseMethodInfo, HarmonyLib.HarmonyPatchType.Prefix, Plugin.HarmonyId);
                }
                if (Plugin.harmony.GetPatchedMethods().Contains(ObstacleSaberSparkleEffectManager_Update.baseMethodInfo))
                {
                    Plugin.harmony.Unpatch(ObstacleSaberSparkleEffectManager_Update.baseMethodInfo, HarmonyLib.HarmonyPatchType.Transpiler, Plugin.HarmonyId);
                }
            }
            Container.BindInterfacesTo<ControllerOffsetPauseViewController>().AsSingle();
        }
    }
}
