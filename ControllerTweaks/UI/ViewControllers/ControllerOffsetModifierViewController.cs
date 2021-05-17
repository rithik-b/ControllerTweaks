using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using ControllerTweaks.Utilities;
using System;
using System.Reflection;
using UnityEngine;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetModifierViewController : ControllerOffsetViewController
    {
        private bool parsed = false;

        internal event Action backButtonClickedEvent;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        public ControllerOffsetModifierViewController(MainSettingsMenuViewController mainSettingsMenuViewController, ControllerOffsetPresetsModalController controllerOffsetPresetsModalController) : base(controllerOffsetPresetsModalController)
        {
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
