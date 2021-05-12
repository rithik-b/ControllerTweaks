using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using ControllerTweaks.Configuration;
using HMUI;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetPresetsModalController : IInitializable, INotifyPropertyChanged
    {
        private bool parsed = false;
        private int selectedIndex = -1;

        [UIComponent("list")]
        public CustomListTableData customListTableData;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform;

        private Vector3 modalPosition;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
            parsed = false;
        }

        private void Parse(RectTransform parent)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetPresetModal.bsml"), parent.gameObject, this);
                modalPosition = modalTransform.localPosition;
                parsed = true;
            }
            modalTransform.SetParent(parent);
            modalTransform.localPosition = modalPosition; // Reset position
        }

        internal void ShowModal(RectTransform parent)
        {
            Parse(parent);
            parserParams.EmitEvent("close-modal");
            parserParams.EmitEvent("open-modal");
            ShowPresets();
        }

        private void ShowPresets()
        {
            customListTableData.tableView.ClearSelection();
            selectedIndex = -1;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyButtonsEnabled)));
            customListTableData.data.Clear();
            foreach (var preset in PluginConfig.Instance.OffsetPresets)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(preset.Key, preset.Value.ToString()));
            }
            customListTableData.tableView.ReloadData();
            customListTableData.tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScrollButtonsEnabled)));
        }

        [UIAction("select-cell")]
        private void OnCellSelect(TableView _, int index)
        {
            selectedIndex = index;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyButtonsEnabled)));
        }

        [UIAction("load-preset")]
        private void LoadPreset()
        {
        }

        [UIAction("save-preset")]
        private void SavePreset()
        {
        }

        [UIAction("delete-preset")]
        private void DeletePreset()
        {
        }

        [UIValue("scroll-buttons-enabled")]
        private bool ScrollButtonsEnabled
        {
            get => customListTableData != null && customListTableData.data.Count > 4;
        }

        [UIValue("modify-buttons-enabled")]
        private bool ModifyButtonsEnabled
        {
            get => selectedIndex != -1;
        }
    }
}
