using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using ControllerTweaks.Configuration;
using ControllerTweaks.Utilities;
using HMUI;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetPresetsModalController : IInitializable, INotifyPropertyChanged
    {
        private bool parsed = false;
        private bool _addButtonActive = true;
        private int selectedIndex = -1;

        private readonly Vector3SO positionOffset;
        private readonly Vector3SO rotationOffset;

        [UIComponent("list")]
        public CustomListTableData customListTableData;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        [UIComponent("modal")]
        internal ModalView modalView;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform;

        private Vector3 modalPosition;

        [UIComponent("keyboard")]
        private ModalView keyboardModalView;

        [UIComponent("keyboard")]
        private readonly RectTransform keyboardModalTransform;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        public event Action offsetPresetLoadedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        public ControllerOffsetPresetsModalController([InjectOptional] SaberManager saberManager, [InjectOptional] MainSettingsMenuViewController mainSettingsMenuViewController)
        {
            if (saberManager != null)
            {
                VRController leftController = saberManager.leftSaber.GetComponentInParent<VRController>();
                VRControllersValueSOOffsets vrControllerTransformOffset = (VRControllersValueSOOffsets)Accessors.VRControllerTransformOffsetAccessor(ref leftController);
                positionOffset = Accessors.PositionOffsetAccessor(ref vrControllerTransformOffset);
                rotationOffset = Accessors.RotationOffsetAccessor(ref vrControllerTransformOffset);
            }
            else if (mainSettingsMenuViewController != null)
            {
                SettingsSubMenuInfo[] settingsSubMenuInfos = Accessors.SettingsSubMenuInfoAccessor(ref mainSettingsMenuViewController);
                foreach (var settingSubMenuInfo in settingsSubMenuInfos)
                {
                    if (settingSubMenuInfo.viewController is ControllersTransformSettingsViewController controllersTransformSettingsViewController)
                    {
                        positionOffset = Accessors.ControllerPositionAccessor(ref controllersTransformSettingsViewController);
                        rotationOffset = Accessors.ControllerRotationAccessor(ref controllersTransformSettingsViewController);
                        break;
                    }
                }
            }
        }

        public void Initialize()
        {
            parsed = false;
        }

        internal void OnActivate()
        {
            if (parsed && modalTransform != null)
            {
                modalTransform.gameObject.SetActive(false);
            }
        }

        internal void OnDeactivate()
        {
            if (parsed && rootTransform != null && modalTransform != null && keyboardModalTransform != null)
            {
                keyboardModalTransform.SetParent(modalTransform);
                modalTransform.SetParent(rootTransform);
            }
        }

        private void Parse(RectTransform parent, bool addButtonActive)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetPresetModal.bsml"), parent.gameObject, this);
                modalPosition = modalTransform.localPosition;
                FieldAccessor<ModalView, bool>.Set(ref keyboardModalView, "_animateParentCanvas", false);
                parsed = true;
            }
            modalTransform.SetParent(parent);
            modalTransform.localPosition = modalPosition;
            FieldAccessor<ModalView, bool>.Set(ref modalView, "_animateParentCanvas", true);
            AddButtonActive = addButtonActive;
        }

        internal void ShowModal(RectTransform parent, bool addButtonActive = true)
        {
            Parse(parent, addButtonActive);
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

        [UIAction("create-preset")]
        private void CreatePreset(string presetName)
        {
            if (presetName != "" && !PluginConfig.Instance.OffsetPresets.ContainsKey(presetName))
            {
                ControllerOffset newPreset = new ControllerOffset();
                PluginConfig.Instance.OffsetPresets.Add(presetName, newPreset);
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(presetName, newPreset.ToString()));
                customListTableData.tableView.ScrollToCellWithIdx(customListTableData.data.Count, TableView.ScrollPositionType.End, true);
                customListTableData.tableView.ReloadData();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScrollButtonsEnabled)));
            }
        }

        [UIAction("load-preset")]
        private void LoadPreset()
        {
            if (selectedIndex != -1)
            {
                ControllerOffset selectedPreset = PluginConfig.Instance.OffsetPresets[customListTableData.data[selectedIndex].text];
                positionOffset.value = new Vector3(Mathf.Clamp(selectedPreset.positionX, -0.1f, 0.1f), Mathf.Clamp(selectedPreset.positionY, -0.1f, 0.1f), Mathf.Clamp(selectedPreset.positionZ, -0.1f, 0.1f));
                rotationOffset.value = new Vector3(Mathf.Clamp(selectedPreset.rotationX, -180f, 180f), Mathf.Clamp(selectedPreset.rotationY, -180f, 180f), Mathf.Clamp(selectedPreset.rotationZ, -180f, 180f));
                offsetPresetLoadedEvent?.Invoke();
                customListTableData.tableView.ClearSelection();
                selectedIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyButtonsEnabled)));
            }
        }

        [UIAction("save-preset")]
        private void SavePreset()
        {
            if (selectedIndex != -1)
            {
                ControllerOffset selectedPreset = new ControllerOffset(positionOffset, rotationOffset);
                PluginConfig.Instance.OffsetPresets[customListTableData.data[selectedIndex].text] = selectedPreset;
                customListTableData.data[selectedIndex].subtext = selectedPreset.ToString();
                customListTableData.tableView.ClearSelection();
                customListTableData.tableView.ReloadData();
                customListTableData.tableView.ScrollToCellWithIdx(selectedIndex, TableView.ScrollPositionType.Center, false);
                selectedIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyButtonsEnabled)));
            }
        }

        [UIAction("delete-preset")]
        private void DeletePreset()
        {
            if (selectedIndex != -1)
            {
                PluginConfig.Instance.OffsetPresets.Remove(customListTableData.data[selectedIndex].text);
                customListTableData.tableView.ClearSelection();
                customListTableData.data.RemoveAt(selectedIndex);
                customListTableData.tableView.ReloadData();
                customListTableData.tableView.ScrollToCellWithIdx(selectedIndex - 1, TableView.ScrollPositionType.Center, false);
                selectedIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyButtonsEnabled)));
            }
        }

        [UIValue("scroll-buttons-enabled")]
        private bool ScrollButtonsEnabled
        {
            get => customListTableData != null && customListTableData.data.Count >= 4;
        }

        [UIValue("add-button-active")]
        private bool AddButtonActive
        {
            get => _addButtonActive;
            set
            {
                _addButtonActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonActive)));
            }
        }

        [UIValue("modify-buttons-enabled")]
        private bool ModifyButtonsEnabled
        {
            get => selectedIndex != -1;
        }
    }
}
