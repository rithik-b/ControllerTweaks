using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using ControllerTweaks.Components;
using ControllerTweaks.Configuration;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage.Components.Settings;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetPauseViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private FloatingScreen floatingScreen;
        private readonly PauseMenuManager pauseMenuManager;
        private readonly ControllerOffsetPresetsModalController controllerOffsetPresetsModalController;

        private VRControllersValueSOOffsets vrControllerTransformOffset;
        private VRController leftController;
        private VRController rightController;

        private Vector3SO positionOffset;
        private Vector3SO rotationOffset;
        private OffsetPreset selectedPreset;

        [UIComponent("position-x-slider")]
        private SliderSetting positionXSlider;

        [UIComponent("position-y-slider")]
        private SliderSetting positionYSlider;

        [UIComponent("position-z-slider")]
        private SliderSetting positionZSlider;

        [UIComponent("rotation-x-slider")]
        private SliderSetting rotationXSlider;

        [UIComponent("rotation-y-slider")]
        private SliderSetting rotationYSlider;

        [UIComponent("rotation-z-slider")]
        private SliderSetting rotationZSlider;

        [UIComponent("left-button")]
        private RectTransform leftButton;

        [UIComponent("right-button")]
        private RectTransform rightButton;

        [UIComponent("show-presets-button")]
        private RectTransform showPresetsButton;

        public static readonly FieldAccessor<VRController, VRControllerTransformOffset>.Accessor VRControllerTransformOffsetAccessor = FieldAccessor<VRController, VRControllerTransformOffset>.GetAccessor("_transformOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor PositionOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_positionOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor RotationOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_rotationOffset");
        
        public static readonly float kPositionStep = 0.1f;
        public static readonly float kRotationStep = 1f;

        public event PropertyChangedEventHandler PropertyChanged;

        public ControllerOffsetPauseViewController(PauseMenuManager pauseMenuManager, ControllerOffsetPresetsModalController controllerOffsetPresetsModalController, SaberManager saberManager)
        {
            this.pauseMenuManager = pauseMenuManager;
            this.controllerOffsetPresetsModalController = controllerOffsetPresetsModalController;
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
        }

        public void Initialize()
        {
            vrControllerTransformOffset = (VRControllersValueSOOffsets)VRControllerTransformOffsetAccessor(ref leftController);
            positionOffset = PositionOffsetAccessor(ref vrControllerTransformOffset);
            rotationOffset = RotationOffsetAccessor(ref vrControllerTransformOffset);

            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(115, 55), true, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            floatingScreen.transform.SetParent(pauseMenuManager.transform.Find("Wrapper").Find("MenuWrapper").Find("Canvas").Find("MainBar").transform);
            floatingScreen.transform.localPosition = new Vector3(0, 35, 0);
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetPauseView.bsml"), floatingScreen.gameObject, this);
        }

        public void Dispose()
        {
            GameObject.Destroy(floatingScreen);
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

        private string PresetFormatter(int index)
        {
            if (index == 0)
            {
                return "Default";
            }
            return PluginConfig.Instance.OffsetPresets.Keys.ToArray()[index - 1];
        }

        [UIAction("show-presets-modal")]
        private void ShowPresetsModal()
        {
            controllerOffsetPresetsModalController.ShowModal(showPresetsButton);
        }

        /*
        private int PresetIndex
        {
            get => PluginConfig.Instance.SelectedPreset;
            set
            {
                PluginConfig.Instance.SelectedPreset = value;

                // Apply preset
                if (value == 0)
                {
                    string filePath = Application.persistentDataPath + "/settings.cfg";
                    string backupFilePath = Application.persistentDataPath + "/settings.cfg.bak";
                    MainSettingsModelSO.Config config = FileHelpers.LoadFromJSONFile<MainSettingsModelSO.Config>(filePath, backupFilePath);
                    positionOffset.value = new Vector3(config.controllerPositionX, config.controllerPositionY, config.controllerPositionZ);
                    rotationOffset.value = new Vector3(config.controllerRotationX, config.controllerRotationY, config.controllerRotationZ);
                }
                else
                {
                    selectedPreset = PluginConfig.Instance.OffsetPresets.Values.ToArray()[PluginConfig.Instance.SelectedPreset - 1];
                    positionOffset.value = new Vector3(selectedPreset.positionX, selectedPreset.positionY, selectedPreset.positionZ);
                    rotationOffset.value = new Vector3(selectedPreset.rotationX, selectedPreset.rotationY, selectedPreset.rotationZ);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PresetIndex)));
            }
        }
        */

        [UIValue("max-preset")]
        private int MaxPreset => PluginConfig.Instance.OffsetPresets.Count;

        [UIValue("position-x")]
        private float PositionX
        {
            get => positionOffset.value.x * 100f;
            set
            {
                positionOffset.value = new Vector3(value / 100f, PositionY / 100f, PositionZ / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionX)));
            }
        }

        [UIValue("position-y")]
        private float PositionY
        {
            get => positionOffset.value.y * 100f;
            set
            {
                positionOffset.value = new Vector3(PositionX / 100f, value / 100f, PositionZ / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionY)));
            }
        }

        [UIValue("position-z")]
        private float PositionZ
        {
            get => positionOffset.value.z * 100f;
            set
            {
                positionOffset.value = new Vector3(PositionX / 100f, PositionY / 100f, value / 100f);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionZ)));
            }
        }

        [UIValue("rotation-x")]
        private float RotationX
        {
            get => rotationOffset.value.x;
            set
            {
                rotationOffset.value = new Vector3(value, RotationY, RotationZ);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationX)));
            }
        }

        [UIValue("rotation-y")]
        private float RotationY
        {
            get => rotationOffset.value.y;
            set
            {
                rotationOffset.value = new Vector3(RotationX, value, RotationZ);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationY)));
            }
        }

        [UIValue("rotation-z")]
        private float RotationZ
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
