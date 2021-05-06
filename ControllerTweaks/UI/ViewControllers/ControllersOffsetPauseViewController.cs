using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using ControllerTweaks.Configuration;
using IPA.Utilities;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ControllersOffsetPauseViewController : IInitializable, INotifyPropertyChanged
    {
        private readonly PauseMenuManager pauseMenuManager;
        private VRControllersValueSOOffsets vrControllerTransformOffset;
        private VRController leftController;
        private VRController rightController;

        private Vector3SO positionOffset;
        private Vector3SO rotationOffset;
        private OffsetPreset selectedPreset;

        public static readonly FieldAccessor<VRController, VRControllerTransformOffset>.Accessor VRControllerTransformOffsetAccessor = FieldAccessor<VRController, VRControllerTransformOffset>.GetAccessor("_transformOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor PositionOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_positionOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor RotationOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_rotationOffset");

        public event PropertyChangedEventHandler PropertyChanged;

        public ControllersOffsetPauseViewController(PauseMenuManager pauseMenuManager, SaberManager saberManager)
        {
            this.pauseMenuManager = pauseMenuManager;
            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>();
        }

        public void Initialize()
        {
            vrControllerTransformOffset = (VRControllersValueSOOffsets)VRControllerTransformOffsetAccessor(ref leftController);
            positionOffset = PositionOffsetAccessor(ref vrControllerTransformOffset);
            rotationOffset = RotationOffsetAccessor(ref vrControllerTransformOffset);

            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetPauseView.bsml"), pauseMenuManager.transform.Find("Wrapper").Find("MenuWrapper").Find("Canvas").Find("MainBar").gameObject, this);

            if (PresetIndex != 0)
            {
                selectedPreset = PluginConfig.Instance.OffsetPresets.Values.ToArray()[PluginConfig.Instance.SelectedPreset - 1];
            }
        }

        [UIAction("preset-formatter")]
        private string PresetFormatter(int index)
        {
            if (index == 0)
            {
                return "Default";
            }
            return PluginConfig.Instance.OffsetPresets.Keys.ToArray()[index - 1];
        }

        [UIValue("offset-preset")]
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

        [UIValue("max-preset")]
        private int MaxPreset => PluginConfig.Instance.OffsetPresets.Count;

        [UIValue("position-x")]
        private float PositionX
        {
            get => positionOffset.value.x * 100f;
            set
            {
                positionOffset.value = new Vector3(value / 100f, PositionY / 100f, PositionZ / 100f);

                if (PresetIndex != 0)
                {
                    selectedPreset.positionX = value / 100f;
                }

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

                if (PresetIndex != 0)
                {
                    selectedPreset.positionY = value / 100f;
                }

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

                if (PresetIndex != 0)
                {
                    selectedPreset.positionZ = value / 100f;
                }

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

                if (PresetIndex != 0)
                {
                    selectedPreset.rotationX = value;
                }

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

                if (PresetIndex != 0)
                {
                    selectedPreset.rotationY = value;
                }

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

                if (PresetIndex != 0)
                {
                    selectedPreset.rotationZ = value;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RotationZ)));
            }
        }
    }
}
