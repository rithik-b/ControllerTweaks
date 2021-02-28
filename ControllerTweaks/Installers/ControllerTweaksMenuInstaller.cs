using ControllerTweaks.UI;
using Zenject;

namespace ControllerTweaks.Installers
{
    class ControllerTweaksMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ModifierViewController>().AsSingle();
        }
    }
}
