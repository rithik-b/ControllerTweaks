using ControllerTweaks.Configuration;
using ControllerTweaks.AffinityPatches;
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

                Container.BindInterfacesAndSelfTo<LocalActiveGameplayManagerStartPatch>().AsSingle();
                Container.BindInterfacesTo<SaberTypePatch>().AsSingle();
                Container.BindInterfacesTo<EffectManagerUpdatePatch>().AsSingle();
            }
        }
    }
}
