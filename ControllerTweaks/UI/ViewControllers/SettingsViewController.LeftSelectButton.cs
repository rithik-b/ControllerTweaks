using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ControllerTweaks.Configuration;
using HMUI;
using System.ComponentModel;

namespace ControllerTweaks.UI
{
    public partial class SettingsViewController
    {
        int selectedLeftTableIndex = -1;
        private string leftToAdd = "Start";

        [UIComponent("left-button-list")]
        public CustomListTableData leftButtonList;

        // Actions

        [UIAction("add-left-clicked")]
        private void AddLeftClicked()
        {
            leftButtonList.data.Add(new CustomListTableData.CustomCellInfo(leftToAdd));
            PluginConfig.Instance.LeftSelectButtons.Add(InputManager.NameToButton[leftToAdd]);
            leftButtonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddLeftInteractable)));
        }

        [UIAction("remove-left-clicked")]
        private void RemoveLeftClicked()
        {
            if (selectedLeftTableIndex != -1)
            {
                leftButtonList.tableView.ClearSelection();
                leftButtonList.data.RemoveAt(selectedLeftTableIndex);
                PluginConfig.Instance.LeftSelectButtons.RemoveAt(selectedLeftTableIndex);
                leftButtonList.tableView.ReloadData();
                selectedLeftTableIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddLeftInteractable)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveLeftInteractable)));
            }
        }

        [UIAction("select-left-list")]
        private void SelectLeft(TableView _, int selectedLeftTableIndex)
        {
            this.selectedLeftTableIndex = selectedLeftTableIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveLeftInteractable)));
        }

        [UIAction("left-to-add-changed")]
        private void leftToAddChanged(string leftToAdd)
        {
            this.leftToAdd = leftToAdd;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddLeftInteractable)));
        }

        // Values

        [UIValue("left-remap-enabled")]
        private bool LeftRemapEnabled
        {
            get => PluginConfig.Instance.LeftSelectRemapEnabled;
            set
            {
                PluginConfig.Instance.LeftSelectRemapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeftRemapEnabled)));
            }
        }

        [UIValue("add-left-interactable")]
        private bool AddLeftInteractable => leftButtonList == null ? true : !leftButtonList.data.Exists(data => data.text == leftToAdd);

        [UIValue("remove-left-interactable")]
        private bool RemoveLeftInteractable => selectedLeftTableIndex != -1;
    }
}
