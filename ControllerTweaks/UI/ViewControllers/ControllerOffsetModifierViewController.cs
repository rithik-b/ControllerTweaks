using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using ControllerTweaks.Utilities;
using System;
using System.Reflection;
using UnityEngine;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetModifierViewController : ControllerOffsetViewController
    {
        private GameplaySetupViewController gameplaySetupViewController;
        private bool parsed = false;
        internal event Action backButtonClickedEvent;

        public ControllerOffsetModifierViewController(GameplaySetupViewController gameplaySetupViewController, MainSettingsMenuViewController mainSettingsMenuViewController, ControllerOffsetPresetsModalController controllerOffsetPresetsModalController, ControllerOffsetSettingsModalController controllerOffsetSettingsModalController) : base(controllerOffsetPresetsModalController, controllerOffsetSettingsModalController)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            SettingsSubMenuInfo[] settingsSubMenuInfos = Accessors.SettingsSubMenuInfoAccessor(ref mainSettingsMenuViewController);
            foreach (var settingSubMenuInfo in settingsSubMenuInfos)
            {
                if (settingSubMenuInfo.viewController is ControllersTransformSettingsViewController controllersTransformSettingsViewController)
                {
                    positionOffset = Accessors.ControllerPositionAccessor(ref controllersTransformSettingsViewController);
                    rotationOffset = Accessors.ControllerRotationAccessor(ref controllersTransformSettingsViewController);
                    break;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            parsed = false;
            gameplaySetupViewController.didActivateEvent += GameplaySetupViewController_didActivateEvent;
            gameplaySetupViewController.didDeactivateEvent += GameplaySetupViewController_didDeactivateEvent;
        }

        public override void Dispose()
        {
            gameplaySetupViewController.didActivateEvent -= GameplaySetupViewController_didActivateEvent;
            gameplaySetupViewController.didDeactivateEvent -= GameplaySetupViewController_didDeactivateEvent;
        }

        private void GameplaySetupViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            controllerOffsetPresetsModalController.OnActivate();
            controllerOffsetSettingsModalController.OnActivate();
        }

        private void GameplaySetupViewController_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            controllerOffsetPresetsModalController.OnDeactivate();
            controllerOffsetSettingsModalController.OnDeactivate();
        }

        private void Parse(RectTransform sibling)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetView.bsml"), sibling.parent.gameObject, this);
                parsed = true;
            }
            rootTransform.SetParent(sibling.parent);
        }

        internal void ShowMenu(RectTransform sibling)
        {
            Parse(sibling);
            rootTransform.gameObject.SetActive(true);
        }

        [UIAction("back-button-clicked")]
        private void BackButtonClicked()
        {
            backButtonClickedEvent?.Invoke();
            rootTransform.gameObject.SetActive(false);
        }
    }
}
