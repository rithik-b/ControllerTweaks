using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ControllerTweaks.HarmonyPatches;
using ControllerTweaks.Interfaces;
using ControllerTweaks.Managers;
using HMUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public abstract class RemapViewController : IInitializable, ISettingsSubviewController, INotifyPropertyChanged
    {
        private IVRPlatformHelper vrPlatformHelper;
        private bool _remapEnabledToggle;

        protected int selectedButtonTableIndex = -1;
        protected string buttonToAdd = "Start";
        private bool parsed;

        [UIComponent("button-list")]
        public CustomListTableData buttonList;

        [UIComponent("root-transform")]
        protected RectTransform rootTransform;

        public event PropertyChangedEventHandler PropertyChanged;

        public RemapViewController(IVRPlatformHelper vrPlatformHelper)
        {
            this.vrPlatformHelper = vrPlatformHelper;
        }

        public void Initialize()
        {
            parsed = false;
            _remapEnabledToggle = ButtonRemapEnabled;
        }

        public void Activate(RectTransform parentTransform)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.RemapView.bsml"), parentTransform.gameObject, this);
            }
            rootTransform.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            rootTransform.gameObject.SetActive(false);
        }

        public void ApplyChanges()
        {
            if (parsed)
            {
                ButtonRemapEnabled = RemapEnabledToggle;
                Buttons.Clear();
                foreach (var cell in buttonList.data)
                {
                    Buttons.Add(ControllerTweaksInputManager.NameToButton[cell.text]);
                }
            }
        }

        #region Actions

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            parsed = true;
            RemapEnabledToggle = ButtonRemapEnabled;
            foreach (var button in Buttons)
            {
                buttonList.data.Add(new CustomListTableData.CustomCellInfo(ControllerTweaksInputManager.ButtonToName[button]));
            }
            buttonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonInteractable)));
            Deactivate();
        }

        [UIAction("add-button-clicked")]
        protected void AddButtonClicked()
        {
            buttonList.data.Add(new CustomListTableData.CustomCellInfo(buttonToAdd));
            buttonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonInteractable)));
        }

        [UIAction("remove-button-clicked")]
        protected void RemoveButtonClicked()
        {
            if (selectedButtonTableIndex != -1)
            {
                buttonList.tableView.ClearSelection();
                buttonList.data.RemoveAt(selectedButtonTableIndex);
                buttonList.tableView.ReloadData();
                selectedButtonTableIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonInteractable)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveButtonInteractable)));
            }
        }

        [UIAction("select-button-list")]
        protected void SelectButton(TableView _, int selectedButtonTableIndex)
        {
            this.selectedButtonTableIndex = selectedButtonTableIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveButtonInteractable)));
        }

        [UIAction("button-to-add-changed")]
        protected void ButtonToAddChanged(string buttonToAdd)
        {
            this.buttonToAdd = buttonToAdd;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonInteractable)));
        }

        #endregion

        #region Values

        [UIValue("remap-enabled")]
        protected bool RemapEnabledToggle
        {
            get => _remapEnabledToggle;
            set
            {
                _remapEnabledToggle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemapEnabledToggle)));
            }
        }

        [UIValue("button-options")]
        protected readonly List<object> buttonOptions = new List<object>{
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

        [UIValue("add-button-interactable")]
        protected bool AddButtonInteractable => buttonList == null ? true : !buttonList.data.Exists(data => data.text == buttonToAdd);

        [UIValue("remove-button-interactable")]
        protected bool RemoveButtonInteractable => selectedButtonTableIndex != -1;

        [UIValue("warning-text")]
        protected string WarningText
        {
            get
            {
                if (vrPlatformHelper.vrPlatformSDK != VRPlatformSDK.Oculus)
                {
                    return "Remap only works for Oculus VR.\nFor SteamVR, use SteamVR's built in button remapper.";
                }
                else if (VRControllersInputManager_MenuButtonDown.failedPatch || VRControllersInputManager_TriggerValue.failedPatch)
                {
                    return "Remap patch failed.\nPlease check for updates for this mod or contact #pc-help in BSMG.";
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Abstract

        protected abstract bool ButtonRemapEnabled { get; set; }
        protected abstract List<OVRInput.Button> Buttons { get; }

        #endregion
    }
}
