using ControllerTweaks.UI;
using Zenject;

namespace ControllerTweaks.Installers
{
    public class ControllerTweaksMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ModifierViewController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ControllerOffsetModifierViewController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ControllerOffsetPresetsModalController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ControllerOffsetSettingsModalController>().AsSingle();
            Container.BindInterfacesTo<SettingsViewController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ButtonSelectionModalController>().AsSingle();
            Container.BindInterfacesTo<PauseRemapViewController>().AsSingle();
            Container.BindInterfacesTo<LeftSelectRemapViewController>().AsSingle();
            Container.BindInterfacesTo<RightSelectRemapViewController>().AsSingle();
        }
    }
}
