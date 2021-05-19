using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using ControllerTweaks.Configuration;
using System;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ModifierViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly ControllerOffsetModifierViewController controllerOffsetModifierViewController;

        private int clicksToChange = 5;
        private string _imagesrc;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        public event PropertyChangedEventHandler PropertyChanged;

        public ModifierViewController(ControllerOffsetModifierViewController controllerOffsetModifierViewController)
        {
            this.controllerOffsetModifierViewController = controllerOffsetModifierViewController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Controller Tweaks", "ControllerTweaks.UI.Views.ModifierView.bsml", this);
            ImageSrc = "ControllerTweaks.Images.ElectroMint_uncropped.png";
            controllerOffsetModifierViewController.backButtonClickedEvent += ControllerOffsetModifierViewController_backButtonClicked;
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab("Controller Tweaks");
            controllerOffsetModifierViewController.backButtonClickedEvent -= ControllerOffsetModifierViewController_backButtonClicked;
        }

        [UIAction("show-offset-menu")]
        private void ShowOffsetMenu()
        {
            controllerOffsetModifierViewController.ShowMenu(rootTransform);
            rootTransform.gameObject.SetActive(false);
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

        private void ControllerOffsetModifierViewController_backButtonClicked()
        {
            rootTransform.gameObject.SetActive(true);
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

        [UIValue("controller-offset-enabled")]
        private bool ControllerOffsetEnabled
        {
            get => PluginConfig.Instance.ShowControllerOffsetInPracticeMode;
            set
            {
                PluginConfig.Instance.ShowControllerOffsetInPracticeMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ControllerOffsetEnabled)));
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
