using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ControllerTweaks.Configuration;
using HMUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTweaks.UI
{
    public partial class SettingsViewController
    {
        int selectedPauseTableIndex = -1;
        private string pauseToAdd = "Start";

        [UIComponent("pause-button-list")]
        public CustomListTableData pauseButtonList;

        // Actions

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

        // Values

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

        [UIValue("add-pause-interactable")]
        private bool AddPauseInteractable => pauseButtonList == null ? true : !pauseButtonList.data.Exists(data => data.text == pauseToAdd);

        [UIValue("remove-pause-interactable")]
        private bool RemovePauseInteractable => selectedPauseTableIndex != -1;
    }
}
