using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using ControllerTweaks.Configuration;
using ControllerTweaks.Utilities;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetPauseViewController : ControllerOffsetViewController
    {
        private FloatingScreen floatingScreen;
        private CanvasGroup floatingScreenCanvasGroup;
        private readonly PauseMenuManager pauseMenuManager;
        private bool practiceMode;

        public enum ScreenPosition
        {
            Top,
            Bottom
        }
        
        public ControllerOffsetPauseViewController([InjectOptional] GameplayCoreSceneSetupData gameplayCoreSceneSetupData, PauseMenuManager pauseMenuManager, SaberManager saberManager, ControllerOffsetPresetsModalController controllerOffsetPresetsModalController, ControllerOffsetSettingsModalController controllerOffsetSettingsModalController) : base(controllerOffsetPresetsModalController, controllerOffsetSettingsModalController)
        {
            if (gameplayCoreSceneSetupData == null || gameplayCoreSceneSetupData?.practiceSettings == null)
            {
                practiceMode = false;
                return;
            }
            practiceMode = true;
            this.pauseMenuManager = pauseMenuManager;
            VRController leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            VRControllersValueSOOffsets vrControllerTransformOffset = (VRControllersValueSOOffsets)Accessors.VRControllerTransformOffsetAccessor(ref leftController);
            positionOffset = Accessors.PositionOffsetAccessor(ref vrControllerTransformOffset);
            rotationOffset = Accessors.RotationOffsetAccessor(ref vrControllerTransformOffset);
        }

        public override void Initialize()
        {
            if (practiceMode)
            {
                base.Initialize();
                floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(115, 55), false, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), curvatureRadius: 150);
                floatingScreen.transform.SetParent(pauseMenuManager.transform.Find("Wrapper").Find("MenuWrapper").Find("Canvas").Find("MainBar"));
                UpdateScreenPosition();
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetView.bsml"), floatingScreen.gameObject, this);
                pauseMenuManager.didPressContinueButtonEvent += PauseMenuManager_didPressContinueButtonEvent;
                controllerOffsetSettingsModalController.screenPositionChanged += UpdateScreenPosition;
            }
        }

        public override void Dispose()
        {
            if (practiceMode)
            {
                pauseMenuManager.didPressContinueButtonEvent -= PauseMenuManager_didPressContinueButtonEvent;
                controllerOffsetSettingsModalController.screenPositionChanged -= UpdateScreenPosition;
            }
        }

        private void PauseMenuManager_didPressContinueButtonEvent()
        {
            controllerOffsetPresetsModalController.OnDeactivate();
            controllerOffsetSettingsModalController.OnDeactivate();
            if (floatingScreenCanvasGroup == null)
            {
                floatingScreenCanvasGroup = floatingScreen.GetComponent<CanvasGroup>();
            }
            if (floatingScreenCanvasGroup != null)
            {
                floatingScreenCanvasGroup.alpha = 1;
            }
        }

        private void UpdateScreenPosition()
        {
            switch (PluginConfig.Instance.ScreenPosition)
            {
                case ScreenPosition.Top:
                    floatingScreen.transform.localPosition = new Vector3(0, 40, 0);
                    break;
                case ScreenPosition.Bottom:
                    floatingScreen.transform.localPosition = new Vector3(-5, -40, 0);
                    break;
            }
        }
    }
}
