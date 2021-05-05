using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Settings;
using ControllerTweaks.Configuration;
using ControllerTweaks.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;

namespace ControllerTweaks.UI
{
    public partial class SettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private IVRPlatformHelper vrPlatformHelper;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewController(IVRPlatformHelper vrPlatformHelper)
        {
            this.vrPlatformHelper = vrPlatformHelper;
            VRControllersInputManager_TriggerValue.vrPlatformHelper = vrPlatformHelper;
            Plugin.ApplyHarmonyPatches();
        }

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

        [UIAction("#apply")]
        public void OnApply()
        {
            Plugin.RemoveHarmonyPatches();
            Plugin.ApplyHarmonyPatches();
        }

        [UIValue("warning-text")]
        private string WarningText
        {
            get
            {
                if (vrPlatformHelper.vrPlatformSDK != VRPlatformSDK.Oculus)
                {
                    return "Remap only works for Oculus VR.\nIf using SteamVR, use SteamVR's built in button remapper.";
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
    }
}
