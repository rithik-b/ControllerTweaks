using ControllerTweaks.Configuration;
using System.Collections.Generic;

namespace ControllerTweaks.UI
{
    public class PauseRemapViewController : RemapViewController
    {
        public override string ToString() => "Pause Button";

        protected override bool ButtonRemapEnabled
        {
            get => PluginConfig.Instance.PauseRemapEnabled;
            set => PluginConfig.Instance.PauseRemapEnabled = value;
        }

        protected override List<OVRInput.Button> Buttons => PluginConfig.Instance.PauseButtons;

        public PauseRemapViewController(ButtonSelectionModalController buttonSelectionModalController, IVRPlatformHelper vrPlatformHelper) : base(buttonSelectionModalController, vrPlatformHelper)
        {
        }
    }
}
