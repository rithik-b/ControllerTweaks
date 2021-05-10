using ControllerTweaks.UI;
using Zenject;

namespace ControllerTweaks.Installers
{
    public class ControllerTweaksMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ModifierViewController>().AsSingle();
            Container.BindInterfacesTo<SettingsViewController>().AsSingle();
            Container.BindInterfacesTo<PauseRemapViewController>().AsSingle();
            Container.BindInterfacesTo<LeftSelectRemapViewController>().AsSingle();
            Container.BindInterfacesTo<RightSelectRemapViewController>().AsSingle();
        }
    }
}
