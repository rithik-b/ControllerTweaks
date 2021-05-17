using IPA.Utilities;

namespace ControllerTweaks.Utilities
{
    public class Accessors
    {
        public static readonly FieldAccessor<MultiplayerLocalActivePlayerGameplayManager, SaberManager>.Accessor SaberManagerAccessor = FieldAccessor<MultiplayerLocalActivePlayerGameplayManager, SaberManager>.GetAccessor("_saberManager");
        public static readonly FieldAccessor<VRController, VRControllerTransformOffset>.Accessor VRControllerTransformOffsetAccessor = FieldAccessor<VRController, VRControllerTransformOffset>.GetAccessor("_transformOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor PositionOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_positionOffset");
        public static readonly FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.Accessor RotationOffsetAccessor = FieldAccessor<VRControllersValueSOOffsets, Vector3SO>.GetAccessor("_rotationOffset");
        public static readonly FieldAccessor<MainSettingsMenuViewController, SettingsSubMenuInfo[]>.Accessor SettingsSubMenuInfoAccessor = FieldAccessor<MainSettingsMenuViewController, SettingsSubMenuInfo[]>.GetAccessor("_settingsSubMenuInfos");
        public static readonly FieldAccessor<ControllersTransformSettingsViewController, Vector3SO>.Accessor ControllerPositionAccessor = FieldAccessor<ControllersTransformSettingsViewController, Vector3SO>.GetAccessor("_controllerPosition");
        public static readonly FieldAccessor<ControllersTransformSettingsViewController, Vector3SO>.Accessor ControllerRotationAccessor = FieldAccessor<ControllersTransformSettingsViewController, Vector3SO>.GetAccessor("_controllerRotation");

    }
}
