using BeatSaberMarkupLanguage.Attributes;
using ControllerTweaks.Components;
using System;
using System.ComponentModel;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage.Components.Settings;
using ControllerTweaks.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        protected readonly ControllerOffsetPresetsModalController controllerOffsetPresetsModalController;
        protected readonly ControllerOffsetSettingsModalController controllerOffsetSettingsModalController;

        protected Vector3SO positionOffset;
        protected Vector3SO rotationOffset;

        private SemaphoreSlim offsetSemaphore;
        private ControllerOffset offsetToApply;
        private SemaphoreSlim waitSemaphore;
        private bool waitAgain;

        [UIComponent("root")]
        protected readonly RectTransform rootTransform;

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

        [UIComponent("show-settings-button")]
        protected readonly RectTransform showSettingsButton;

        public static readonly float kPositionStep = 0.1f;
        public static readonly float kRotationStep = 1f;

        public event PropertyChangedEventHandler PropertyChanged;

        public ControllerOffsetViewController(ControllerOffsetPresetsModalController controllerOffsetPresetsModalController, ControllerOffsetSettingsModalController controllerOffsetSettingsModalController)
        {
            this.controllerOffsetPresetsModalController = controllerOffsetPresetsModalController;
            this.controllerOffsetSettingsModalController = controllerOffsetSettingsModalController;
        }

        public virtual void Initialize()
        {
            offsetSemaphore = new SemaphoreSlim(1, 1);
            offsetToApply = null;
            waitSemaphore = new SemaphoreSlim(1, 1);
            waitAgain = false;
            controllerOffsetPresetsModalController.offsetPresetLoadedEvent += UpdateOffsetValues;
        }

        public virtual void Dispose()
        {
            controllerOffsetPresetsModalController.offsetPresetLoadedEvent -= UpdateOffsetValues;
        }

        private async Task SetOffsetDelay(ControllerOffset.OffsetType offsetType, float value)
        {
            await offsetSemaphore.WaitAsync();
            if (offsetToApply == null)
            {
                offsetToApply = new ControllerOffset(positionOffset, rotationOffset);
                offsetToApply.SetValue(offsetType, value);
                offsetSemaphore.Release();
                do
                {
                    await waitSemaphore.WaitAsync();
                    waitAgain = false;
                    waitSemaphore.Release();
                    await Task.Delay(1000);
                } while (waitAgain);
                await offsetSemaphore.WaitAsync();
                positionOffset.value = new Vector3(Mathf.Clamp(offsetToApply.positionX, -0.1f, 0.1f), Mathf.Clamp(offsetToApply.positionY, -0.1f, 0.1f), Mathf.Clamp(offsetToApply.positionZ, -0.1f, 0.1f));
                rotationOffset.value = new Vector3(Mathf.Clamp(offsetToApply.rotationX, -180f, 180f), Mathf.Clamp(offsetToApply.rotationY, -180f, 180f), Mathf.Clamp(offsetToApply.rotationZ, -180f, 180f));
                offsetToApply = null;
                offsetSemaphore.Release();
                UpdateOffsetValues();
            }
            else
            {
                offsetToApply.SetValue(offsetType, value);
                offsetSemaphore.Release();
                await waitSemaphore.WaitAsync();
                waitAgain = true;
                waitSemaphore.Release();
            }
        }

        private void UpdateOffsetValues()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));
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
            controllerOffsetPresetsModalController.ShowModal(showPresetsButton, IsModifierController);
        }

        [UIAction("show-settings-modal")]
        protected void ShowSettingsModal()
        {
            controllerOffsetSettingsModalController.ShowModal(showSettingsButton);
        }

        [UIValue("back-button-active")]
        protected bool IsModifierController => this is ControllerOffsetModifierViewController;

        [UIValue("position-x")]
        protected float PositionX
        {
            get => positionOffset.value.x * 100f;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.PositionX, value / 100f));
                }
                else
                {
                    positionOffset.value = new Vector3(value / 100f, PositionY / 100f, PositionZ / 100f);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
                }
            }
        }

        [UIValue("position-y")]
        protected float PositionY
        {
            get => positionOffset.value.y * 100f;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.PositionY, value / 100f));
                }
                else
                {
                    positionOffset.value = new Vector3(PositionX / 100f, value / 100f, PositionZ / 100f);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
                }
            }
        }

        [UIValue("position-z")]
        protected float PositionZ
        {
            get => positionOffset.value.z * 100f;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.PositionZ, value / 100f));
                }
                else
                {
                    positionOffset.value = new Vector3(PositionX / 100f, PositionY / 100f, value / 100f);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
                }
            }
        }

        [UIValue("rotation-x")]
        protected float RotationX
        {
            get => rotationOffset.value.x;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.RotationX, value));
                }
                else
                {
                    rotationOffset.value = new Vector3(value, RotationY, RotationZ);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
                }
            }
        }

        [UIValue("rotation-y")]
        protected float RotationY
        {
            get => rotationOffset.value.y;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.RotationY, value));
                }
                else
                {
                    rotationOffset.value = new Vector3(RotationX, value, RotationZ);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
                }
            }
        }

        [UIValue("rotation-z")]
        protected float RotationZ
        {
            get => rotationOffset.value.z;
            set
            {
                if (PluginConfig.Instance.OffsetApplyDelayEnabled)
                {
                    Task.Run(() => SetOffsetDelay(ControllerOffset.OffsetType.RotationZ, value));
                }
                else
                {
                    rotationOffset.value = new Vector3(RotationX, RotationY, value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));
                }
            }
        }
    }
}