using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using ControllerTweaks.Utilities;
using System.Reflection;
using UnityEngine;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetPauseViewController : ControllerOffsetViewController
    {
        private FloatingScreen floatingScreen;
        private readonly PauseMenuManager pauseMenuManager;
        
        public ControllerOffsetPauseViewController(PauseMenuManager pauseMenuManager, SaberManager saberManager, ControllerOffsetPresetsModalController controllerOffsetPresetsModalController) : base(controllerOffsetPresetsModalController)
        {
            this.pauseMenuManager = pauseMenuManager;
            VRController leftController = saberManager?.leftSaber.GetComponentInParent<VRController>();
            VRControllersValueSOOffsets vrControllerTransformOffset = (VRControllersValueSOOffsets)Accessors.VRControllerTransformOffsetAccessor(ref leftController);
            positionOffset = Accessors.PositionOffsetAccessor(ref vrControllerTransformOffset);
            rotationOffset = Accessors.RotationOffsetAccessor(ref vrControllerTransformOffset);
        }

        public override void Initialize()
        {
            base.Initialize();
            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(115, 55), true, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            floatingScreen.transform.SetParent(pauseMenuManager.transform.Find("Wrapper").Find("MenuWrapper").Find("Canvas").Find("MainBar").transform);
            floatingScreen.transform.localPosition = new Vector3(0, 35, 0);
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetView.bsml"), floatingScreen.gameObject, this);
        }
    }
}
