using ControllerTweaks.Configuration;
using System.Collections.Generic;

namespace ControllerTweaks.UI
{
    public class RightSelectRemapViewController : RemapViewController
    {
        public override string ToString() => "Right Select Button";

        protected override bool ButtonRemapEnabled
        {
            get => PluginConfig.Instance.RightSelectRemapEnabled;
            set => PluginConfig.Instance.RightSelectRemapEnabled = value;
        }

        protected override List<OVRInput.Button> Buttons => PluginConfig.Instance.RightSelectButtons;

        public RightSelectRemapViewController(ButtonSelectionModalController buttonSelectionModalController, IVRPlatformHelper vrPlatformHelper) : base(buttonSelectionModalController, vrPlatformHelper)
        {
        }
    }
}
