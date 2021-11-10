using ControllerTweaks.AffinityPatches;
using ControllerTweaks.Configuration;
using Zenject;

namespace ControllerTweaks.Installers
{
    public class ControllerTweaksRemapInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (PluginConfig.Instance.PauseRemapEnabled)
            {
                Container.BindInterfacesAndSelfTo<MenuButtonPatch>().AsSingle();
                Container.BindInterfacesAndSelfTo<MenuButtonDownPatch>().AsSingle();
            }

            // Disable select button mapping if nothing mapped
            if (PluginConfig.Instance.LeftSelectRemapEnabled && PluginConfig.Instance.LeftSelectButtons.Count == 0)
            {
                PluginConfig.Instance.LeftSelectRemapEnabled = false; 
            }
            if (PluginConfig.Instance.RightSelectRemapEnabled && PluginConfig.Instance.RightSelectButtons.Count == 0)
            {
                PluginConfig.Instance.RightSelectRemapEnabled = false;
            }

            if (PluginConfig.Instance.LeftSelectRemapEnabled || PluginConfig.Instance.RightSelectRemapEnabled)
            {
                Container.BindInterfacesAndSelfTo<TriggerValuePatch>().AsSingle();
            }
        }
    }
}
