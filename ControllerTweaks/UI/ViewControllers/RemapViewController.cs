using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ControllerTweaks.Interfaces;
using ControllerTweaks.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public abstract class RemapViewController : IInitializable, IDisposable, ISettingsSubviewController, INotifyPropertyChanged
    {
        private readonly ButtonSelectionModalController buttonSelectionModalController;
        private readonly IVRPlatformHelper vrPlatformHelper;

        private bool _remapEnabledToggle;

        protected int selectedButtonTableIndex = -1;
        private bool parsed;

        [UIComponent("button-list")]
        protected CustomListTableData buttonList;

        [UIComponent("root-transform")]
        protected RectTransform rootTransform;

        public event PropertyChangedEventHandler PropertyChanged;

        public RemapViewController(ButtonSelectionModalController buttonSelectionModalController, IVRPlatformHelper vrPlatformHelper)
        {
            this.buttonSelectionModalController = buttonSelectionModalController;
            this.vrPlatformHelper = vrPlatformHelper;
        }

        public void Initialize()
        {
            parsed = false;
            _remapEnabledToggle = ButtonRemapEnabled;

            buttonSelectionModalController.AddButtonClickedEvent += ButtonSelectionModalController_AddButtonClickedEvent;
        }

        public void Dispose()
        {
            buttonSelectionModalController.AddButtonClickedEvent -= ButtonSelectionModalController_AddButtonClickedEvent;
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
                    Buttons.Add(ControllerTweaksInputHelper.NameToButton[cell.text]);
                }
            }
        }

        private void ButtonSelectionModalController_AddButtonClickedEvent(string buttonToAdd, Type callingType)
        {
            if (parsed && callingType == GetType())
            {
                buttonList.data.Add(new CustomListTableData.CustomCellInfo(buttonToAdd));
                buttonList.tableView.ReloadData();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WarningText)));
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
                buttonList.data.Add(new CustomListTableData.CustomCellInfo(ControllerTweaksInputHelper.ButtonToName[button]));
            }
            buttonList.tableView.ReloadData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WarningText)));
            Deactivate();
        }

        [UIAction("add-button-clicked")]
        protected void AddButtonClicked() => buttonSelectionModalController.ShowModal(rootTransform, GetType(), buttonList.data);

        [UIAction("remove-button-clicked")]
        protected void RemoveButtonClicked()
        {
            if (selectedButtonTableIndex != -1)
            {
                buttonList.tableView.ClearSelection();
                buttonList.data.RemoveAt(selectedButtonTableIndex);
                buttonList.tableView.ReloadData();
                selectedButtonTableIndex = -1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveButtonInteractable)));
            }
        }

        [UIAction("select-button-list")]
        protected void SelectButton(TableView _, int selectedButtonTableIndex)
        {
            this.selectedButtonTableIndex = selectedButtonTableIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemoveButtonInteractable)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WarningText)));
            }
        }

        [UIValue("remove-button-interactable")]
        protected bool RemoveButtonInteractable => selectedButtonTableIndex != -1;

        [UIValue("warning-text")]
        protected string WarningText
        {
            get
            {
                if (vrPlatformHelper.vrPlatformSDK != VRPlatformSDK.Oculus)
                {
                    return "<color=red>Remap only works for Oculus VR.\nFor SteamVR, use SteamVR's built in button remapper.</color>";
                }
                if (RemapEnabledToggle && buttonList?.data.Count == 0)
                {
                    return "Make sure you click \"+\" to bind a button.";
                }
                return "";
            }
        }

#endregion

#region Abstract

        protected abstract bool ButtonRemapEnabled { get; set; }
        protected abstract List<OVRInput.Button> Buttons { get; }

#endregion
    }
}
