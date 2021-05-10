using ControllerTweaks.Managers;
using Zenject;

namespace ControllerTweaks.Installers
{
    public class ControllerTweaksApplicationInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ControllerTweaksInputManager>().AsSingle();
        }
    }
}
