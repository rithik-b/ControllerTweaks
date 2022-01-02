using ControllerTweaks.Configuration;
using ControllerTweaks.UI;
using Zenject;

namespace ControllerTweaks.Installers
{
    internal class ControllerTweaksStandardInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (PluginConfig.Instance.ShowControllerOffsetInPracticeMode)
            {
                Container.BindInterfacesTo<ControllerOffsetPauseViewController>().AsSingle();
                Container.BindInterfacesAndSelfTo<ControllerOffsetPresetsModalController>().AsSingle();
                Container.BindInterfacesAndSelfTo<ControllerOffsetSettingsModalController>().AsSingle();
            }
        }
    }
}
