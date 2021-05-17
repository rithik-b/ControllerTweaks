using BeatSaberMarkupLanguage.Attributes;
using ControllerTweaks.Components;
using System;
using System.ComponentModel;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage.Components.Settings;
using ControllerTweaks.Utilities;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly ControllerOffsetPresetsModalController controllerOffsetPresetsModalController;

        protected Vector3SO positionOffset;
        protected Vector3SO rotationOffset;

        [UIComponent("position-x-slider")]
        protected readonly SliderSetting positionXSlider;

        [UIComponent("position-y-slider")]
        protected readonly SliderSetting positionYSlider;

        [UIComponent("position-z-slider")]
        protected readonly SliderSetting positionZSlider;

        [UIComponent("rotation-x-slider")]
        protected readonly SliderSetting rotationXSlider;

        [UIComponent("rotation-y-slider")]
        protected readonly SliderSetting rotationYSlider;

        [UIComponent("rotation-z-slider")]
        protected readonly SliderSetting rotationZSlider;

        [UIComponent("left-button")]
        protected readonly RectTransform leftButton;

        [UIComponent("right-button")]
        protected readonly RectTransform rightButton;

        [UIComponent("show-presets-button")]
        protected readonly RectTransform showPresetsButton;

        public static readonly float kPositionStep = 0.1f;
        public static readonly float kRotationStep = 1f;

        public event PropertyChangedEventHandler PropertyChanged;

        public ControllerOffsetViewController(ControllerOffsetPresetsModalController controllerOffsetPresetsModalController)
        {
            this.controllerOffsetPresetsModalController = controllerOffsetPresetsModalController;
        }

        public virtual void Initialize()
        {
            controllerOffsetPresetsModalController.offsetPresetLoadedEvent += ControllerOffsetPresetsModalController_offsetPresetLoadedEvent;
        }

        public void Dispose()
        {
            controllerOffsetPresetsModalController.offsetPresetLoadedEvent -= ControllerOffsetPresetsModalController_offsetPresetLoadedEvent;
        }

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), positionXSlider, kPositionStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), positionYSlider, kPositionStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), positionZSlider, kPositionStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), rotationXSlider, kRotationStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), rotationYSlider, kRotationStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), rotationZSlider, kRotationStep);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }

        [UIAction("show-presets-modal")]
        protected void ShowPresetsModal()
        {
            controllerOffsetPresetsModalController.ShowModal(showPresetsButton, false);
        }

        private void ControllerOffsetPresetsModalController_offsetPresetLoadedEvent()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));
        }

        [UIValue("back-button-active")]
        protected bool BackButtonActive => this is ControllerOffsetModifierViewController;

        [UIValue("position-x")]
        protected float PositionX
        {
            get => positionOffset.value.x * 100f;
            set
            {
                positionOffset.value = new Vector3(value / 100f, PositionY / 100f, PositionZ / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
            }
        }

        [UIValue("position-y")]
        protected float PositionY
        {
            get => positionOffset.value.y * 100f;
            set
            {
                positionOffset.value = new Vector3(PositionX / 100f, value / 100f, PositionZ / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
            }
        }

        [UIValue("position-z")]
        protected float PositionZ
        {
            get => positionOffset.value.z * 100f;
            set
            {
                positionOffset.value = new Vector3(PositionX / 100f, PositionY / 100f, value / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
            }
        }

        [UIValue("rotation-x")]
        protected float RotationX
        {
            get => rotationOffset.value.x;
            set
            {
                rotationOffset.value = new Vector3(value, RotationY, RotationZ);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
            }
        }

        [UIValue("rotation-y")]
        protected float RotationY
        {
            get => rotationOffset.value.y;
            set
            {
                rotationOffset.value = new Vector3(RotationX, value, RotationZ);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
            }
        }

        [UIValue("rotation-z")]
        protected float RotationZ
        {
            get => rotationOffset.value.z;
            set
            {
                rotationOffset.value = new Vector3(RotationX, RotationY, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));
            }
        }
    }
}
