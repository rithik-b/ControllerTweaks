using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using ControllerTweaks.Interfaces;
using HMUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class SettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private int currentSubviewIndex = 0;

        [UIValue("settings-subviews")]
        private List<object> settingsSubviewControllers = new List<object>();

        [UIComponent("root-transform")]
        private RectTransform rootTransform;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewController(List<ISettingsSubviewController> settingsSubviewControllers)
        {
            foreach (var subviewController in settingsSubviewControllers)
            {
                this.settingsSubviewControllers.Add(subviewController);
            }
        }

        public void Initialize()
        {
            BSMLSettings.instance.AddSettingsMenu("Controller Tweaks", "ControllerTweaks.UI.Views.SettingsView.bsml", this);
        }

        public void Dispose() => BSMLSettings.instance?.RemoveSettingsMenu(this);

        [UIAction("#post-parse")]
        private void PostParse()
        {
            if (settingsSubviewControllers.Count >= 1)
            {
                ((ISettingsSubviewController)settingsSubviewControllers[currentSubviewIndex]).Activate(rootTransform);
            }
        }

        [UIAction("settings-subview-selected")]
        private void SettingsSubviewSelected(SegmentedControl _, int index)
        {
            ((ISettingsSubviewController)settingsSubviewControllers[currentSubviewIndex]).Deactivate();
            ((ISettingsSubviewController)settingsSubviewControllers[index]).Activate(rootTransform);
            currentSubviewIndex = index;
        }

        [UIAction("#apply")]
        public void OnApply()
        {
            foreach (var subviewController in settingsSubviewControllers)
            {
                ((ISettingsSubviewController)subviewController).ApplyChanges();
            }
        }
    }
}
