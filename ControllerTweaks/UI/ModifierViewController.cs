using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using ControllerTweaks.Configuration;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    class ModifierViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private int clicksToChange = 5;
        private string _imagesrc;

        [UIComponent("swap-checkbox")]
        private ToggleSetting swapCheckbox;

        [UIValue("imagesrc")]
        private string imagesrc
        {
            get => _imagesrc;
            set
            {
                _imagesrc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(imagesrc)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Controller Tweaks", "ControllerTweaks.UI.ModifierView.bsml", this);
            imagesrc = "ControllerTweaks.Images.ElectroCute.png";
        }

        public void Dispose()
        {
            swapCheckbox.ReceiveValue();
            GameplaySetup.instance.RemoveTab("Controller Tweaks");
        }

        [UIAction("image-click")]
        private void ImageClick()
        {
            clicksToChange--;
            if (clicksToChange == 0)
            {
                clicksToChange = 5;
                if (imagesrc == "ControllerTweaks.Images.ElectroCute.png")
                {
                    imagesrc = "ControllerTweaks.Images.ElectroMint_uncropped.png";
                }
                else
                {
                    imagesrc = "ControllerTweaks.Images.ElectroCute.png";
                }
            }
        }

        [UIValue("controller-swap-enabled")]
        private bool ControllerTweaksEnabled
        {
            get { return PluginConfig.Instance.ControllerSwapEnabled; }
            set
            {
                PluginConfig.Instance.ControllerSwapEnabled = value;
            }
        }
    }
}
