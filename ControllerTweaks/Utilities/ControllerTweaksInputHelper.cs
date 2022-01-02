using System.Collections.Generic;

namespace ControllerTweaks.Utilities
{
    public class ControllerTweaksInputHelper
    {
        public static readonly Dictionary<string, OVRInput.Button> NameToButton = new Dictionary<string, OVRInput.Button>
        {
            { "Start", OVRInput.Button.Start },
            { "Left Stick", OVRInput.Button.PrimaryThumbstick },
            { "Right Stick", OVRInput.Button.SecondaryThumbstick },
            { "Left Trigger", OVRInput.Button.PrimaryIndexTrigger },
            { "Right Trigger", OVRInput.Button.SecondaryIndexTrigger },
            { "Left Grip", OVRInput.Button.PrimaryHandTrigger },
            { "Right Grip", OVRInput.Button.SecondaryHandTrigger },
            { "A", OVRInput.Button.One },
            { "B", OVRInput.Button.Two },
            { "X", OVRInput.Button.Three },
            { "Y", OVRInput.Button.Four }
        };

        public static Dictionary<OVRInput.Button, string> ButtonToName = new Dictionary<OVRInput.Button, string>
        {
            { OVRInput.Button.Start, "Start" },
            { OVRInput.Button.PrimaryThumbstick, "Left Stick" },
            { OVRInput.Button.SecondaryThumbstick, "Right Stick" },
            { OVRInput.Button.PrimaryIndexTrigger, "Left Trigger" },
            { OVRInput.Button.SecondaryIndexTrigger, "Right Trigger" },
            { OVRInput.Button.PrimaryHandTrigger, "Left Grip" },
            { OVRInput.Button.SecondaryHandTrigger, "Right Grip" },
            { OVRInput.Button.One, "A" },
            { OVRInput.Button.Two, "B" },
            { OVRInput.Button.Three, "X" },
            { OVRInput.Button.Four, "Y" }
        };
    }
}
