using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ControllerTweaks.UI
{
    public class ButtonSelectionModalController : INotifyPropertyChanged
    {
        private bool parsed = false;
        private string buttonToAdd = "None";
        private Type callingType;
        public event Action<string, Type> AddButtonClickedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        [UIComponent("dropdown-list")]
        private readonly DropDownListSetting dropDownListSetting;

        [UIComponent("dropdown-list")]
        private readonly Transform dropdownListTransform;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform;

        private Vector3 modalPosition;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        private void Parse(RectTransform parent)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ButtonSelectionModal.bsml"), parent.gameObject, this);
                modalPosition = modalTransform.localPosition;
                dropdownListTransform.Find("DropdownTableView").GetComponent<ModalView>().SetField("_animateParentCanvas", false);
                parsed = true;
            }
            modalTransform.SetParent(parent);
            modalTransform.localPosition = modalPosition;
        }

        internal void ShowModal(RectTransform parent, Type callingType, List<CustomListTableData.CustomCellInfo> customCellInfos)
        {
            Parse(parent);
            this.callingType = callingType;

            dropDownListSetting.values = buttonOptions.Except(customCellInfos.Select(c => c.text)).ToList();
            dropDownListSetting.UpdateChoices();
            dropDownListSetting.dropdown.SelectCellWithIdx(0);
            ButtonToAddChanged(buttonToAdd);

            parserParams.EmitEvent("close-modal");
            parserParams.EmitEvent("open-modal");
        }

        internal void OnDeactivate()
        {
            if (parsed && rootTransform != null && modalTransform != null)
            {
                modalTransform.SetParent(rootTransform);
                modalTransform.gameObject.SetActive(false);
            }
        }

        [UIAction("add-button-clicked")]
        private void AddButtonClicked() => AddButtonClickedEvent?.Invoke(buttonToAdd, callingType);

        [UIAction("button-to-add-changed")]
        private void ButtonToAddChanged(string buttonToAdd)
        {
            this.buttonToAdd = buttonToAdd;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddButtonInteractable)));
        }

        [UIValue("add-button-interactable")]
        private bool AddButtonInteractable => buttonToAdd != "None";

        [UIValue("button-options")]
        private readonly List<object> buttonOptions = new List<object>{
            "None",
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
    }
}
