using ControllerTweaks.Configuration;
using Zenject;

namespace ControllerTweaks.Installers
{
    class ControllerTweaksGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (PluginConfig.Instance.ControllerSwapEnabled)
            {
                Container.BindInterfacesTo<ControllerSwapper>().AsSingle();
            }
        }
    }
}
