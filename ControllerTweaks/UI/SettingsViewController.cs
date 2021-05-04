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
    class SettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
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

        int selectedPauseTableIndex = -1;

        private string pauseToAdd = "Start";

        [UIComponent("pause-button-list")]
        public CustomListTableData pauseButtonList;

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

        [UIAction("add-pause-clicked")]
        private void AddPauseClicked()
        {
            pauseButtonList.data.Add(new CustomListTableData.CustomCellInfo(pauseToAdd));
            PluginConfig.Instance.PauseButtons.Add(InputManager.NameToButton[pauseToAdd]);
            pauseButtonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddPauseInteractable)));
        }

        [UIAction("remove-pause-clicked")]
        private void RemovePauseClicked()
        {
            if (selectedPauseTableIndex != -1)
            {
                pauseButtonList.tableView.ClearSelection();
                pauseButtonList.data.RemoveAt(selectedPauseTableIndex);
                PluginConfig.Instance.PauseButtons.RemoveAt(selectedPauseTableIndex);
                pauseButtonList.tableView.ReloadData();
                selectedPauseTableIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddPauseInteractable)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemovePauseInteractable)));
            }
        }

        [UIAction("select-pause-list")]
        private void Select(TableView _, int selectedPauseTableIndex)
        {
            this.selectedPauseTableIndex = selectedPauseTableIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemovePauseInteractable)));
        }

        [UIAction("pause-to-add-changed")]
        private void PauseToAddChanged(string selectedPause)
        {
            this.pauseToAdd = selectedPause;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddPauseInteractable)));
        }

        [UIValue("add-pause-interactable")]
        private bool AddPauseInteractable => pauseButtonList == null ? true : !pauseButtonList.data.Exists(data => data.text == pauseToAdd);

        [UIValue("remove-pause-interactable")]
        private bool RemovePauseInteractable => selectedPauseTableIndex != -1;

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
