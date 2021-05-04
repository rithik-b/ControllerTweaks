using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Settings;
using ControllerTweaks.Configuration;
using HMUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;

namespace ControllerTweaks.UI
{
    public partial class SettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [UIValue("button-options")]
        private readonly List<object> buttonOptions = new List<object>{
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
        public void Dispose() => BSMLSettings.instance?.RemoveSettingsMenu(this);

        [UIAction("#post-parse")]
        private void PostParse()
        {
            foreach (var pauseButton in PluginConfig.Instance.PauseButtons)
            {
                pauseButtonList.data.Add(new CustomListTableData.CustomCellInfo(InputManager.ButtonToName[pauseButton]));
            }
            pauseButtonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddPauseInteractable)));
        }

        [UIAction("#apply")]
        public void OnApply()
        {
            Plugin.RemoveHarmonyPatches();
            Plugin.ApplyHarmonyPatches();
        }

        [UIAction("add-select-clicked")]
        private void AddSelectClicked()
        {

        }

        [UIAction("remove-select-clicked")]
        private void RemoveSelectClicked()
        {

        }

        [UIValue("select-remap-enabled")]
        private bool SelectRemapEnabled
        {
            get => PluginConfig.Instance.LeftSelectRemapEnabled;
            set
            {
                PluginConfig.Instance.LeftSelectRemapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectRemapEnabled)));
            }
        }
    }
}
