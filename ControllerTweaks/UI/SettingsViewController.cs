using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using ControllerTweaks.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ControllerTweaks.UI
{
    class SettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [UIValue("button-options")]
        private List<object> buttonOptions = new List<object>{
            "Start",
            "Left Stick",
            "Right Stick",
            "Left Trigger",
            "Right Trigger",
            "Left Grip",
            "Right Grip",
            "A",
            "B",
            "X",
            "Y"
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize() => BSMLSettings.instance.AddSettingsMenu("Controller Tweaks", "ControllerTweaks.UI.SettingsView.bsml", this);
        public void Dispose() => BSMLSettings.instance.RemoveSettingsMenu(this);

        [UIValue("select-remap-enabled")]
        private bool SelectRemapEnabled
        {
            get => PluginConfig.Instance.SelectRemapEnabled;
            set
            {
                PluginConfig.Instance.SelectRemapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectRemapEnabled)));
            }
        }

        [UIValue("pause-remap-enabled")]
        private bool PauseRemapEnabled
        {
            get => PluginConfig.Instance.PauseRemapEnabled;
            set
            {
                PluginConfig.Instance.PauseRemapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PauseRemapEnabled)));
            }
        }
    }
}
