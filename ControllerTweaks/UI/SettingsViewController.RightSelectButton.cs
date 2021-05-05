using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ControllerTweaks.Configuration;
using HMUI;
using System.ComponentModel;

namespace ControllerTweaks.UI
{
    public partial class SettingsViewController
    {
        int selectedRightTableIndex = -1;
        private string rightToAdd = "Start";

        [UIComponent("right-button-list")]
        public CustomListTableData rightButtonList;

        // Actions

        [UIAction("add-right-clicked")]
        private void AddRightClicked()
        {
            rightButtonList.data.Add(new CustomListTableData.CustomCellInfo(rightToAdd));
            PluginConfig.Instance.RightSelectButtons.Add(InputManager.NameToButton[rightToAdd]);
            rightButtonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddRightInteractable)));
        }

        [UIAction("remove-right-clicked")]
        private void RemoveRightClicked()
        {
            if (selectedRightTableIndex != -1)
            {
                rightButtonList.tableView.ClearSelection();
                rightButtonList.data.RemoveAt(selectedRightTableIndex);
                PluginConfig.Instance.RightSelectButtons.RemoveAt(selectedRightTableIndex);
                rightButtonList.tableView.ReloadData();
                selectedRightTableIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddRightInteractable)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveRightInteractable)));
            }
        }

        [UIAction("select-right-list")]
        private void SelectRight(TableView _, int selectedRightTableIndex)
        {
            this.selectedRightTableIndex = selectedRightTableIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveRightInteractable)));
        }

        [UIAction("right-to-add-changed")]
        private void rightToAddChanged(string rightToAdd)
        {
            this.rightToAdd = rightToAdd;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddRightInteractable)));
        }

        // Values

        [UIValue("right-remap-enabled")]
        private bool RightRemapEnabled
        {
            get => PluginConfig.Instance.RightSelectRemapEnabled;
            set
            {
                PluginConfig.Instance.RightSelectRemapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RightRemapEnabled)));
            }
        }

        [UIValue("add-right-interactable")]
        private bool AddRightInteractable => rightButtonList == null ? true : !rightButtonList.data.Exists(data => data.text == rightToAdd);

        [UIValue("remove-right-interactable")]
        private bool RemoveRightInteractable => selectedRightTableIndex != -1;
    }
}
