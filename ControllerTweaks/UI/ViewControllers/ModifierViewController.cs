using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using ControllerTweaks.Configuration;
using ControllerTweaks.HarmonyPatches;
using System;
using System.ComponentModel;
using Zenject;

namespace ControllerTweaks.UI
{
    class ModifierViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private int clicksToChange = 5;
        private string _imagesrc;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Controller Tweaks", "ControllerTweaks.UI.Views.ModifierView.bsml", this);
            ImageSrc = "ControllerTweaks.Images.ElectroMint_uncropped.png";
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab("Controller Tweaks");
        }

        [UIAction("image-click")]
        private void ImageClick()
        {
            clicksToChange--;
            if (clicksToChange == 0)
            {
                clicksToChange = 5;
                if (ImageSrc == "ControllerTweaks.Images.ElectroMint_uncropped.png")
                {
                    ImageSrc = "ControllerTweaks.Images.ElectroCute.png";
                }
                else
                {
                    ImageSrc = "ControllerTweaks.Images.ElectroMint_uncropped.png";
                }
            }
        }

        [UIValue("controller-swap-enabled")]
        private bool ControllerSwapEnabled
        {
            get => PluginConfig.Instance.ControllerSwapEnabled;
            set
            {
                PluginConfig.Instance.ControllerSwapEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerSwapEnabled)));
            }
        }

        [UIValue("imagesrc")]
        private string ImageSrc
        {
            get => _imagesrc;
            set
            {
                _imagesrc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSrc)));
            }
        }
    }
}
