using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ControllerTweaks.Installers
{
    class ControllerTweaksGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ControllerSwapper>().AsSingle();
        }
    }
}
