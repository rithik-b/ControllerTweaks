using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTweaks
{
    public class InputManager
    {
        public class ButtonInput
        {
            public string buttonName;
            public OVRInput.Button button;
            public OVRInput.Controller controller;

            public ButtonInput(string buttonName, OVRInput.Button button, bool combined)
            {
                this.buttonName = buttonName;
                this.button = button;

                if (combined)
                {
                    
                }
            }
        }
    }
}
