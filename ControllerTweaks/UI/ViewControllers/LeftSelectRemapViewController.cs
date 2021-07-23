using ControllerTweaks.Configuration;
using System.Collections.Generic;

namespace ControllerTweaks.UI
{
    public class LeftSelectRemapViewController : RemapViewController
    {
        public override string ToString() => "Left Select Button";

        protected override bool ButtonRemapEnabled
        {
            get => PluginConfig.Instance.LeftSelectRemapEnabled;
            set => PluginConfig.Instance.LeftSelectRemapEnabled = value;
        }

        protected override List<OVRInput.Button> Buttons => PluginConfig.Instance.LeftSelectButtons;

        public LeftSelectRemapViewController(ButtonSelectionModalController buttonSelectionModalController, IVRPlatformHelper vrPlatformHelper) : base(buttonSelectionModalController, vrPlatformHelper)
        {
        }
    }
}
