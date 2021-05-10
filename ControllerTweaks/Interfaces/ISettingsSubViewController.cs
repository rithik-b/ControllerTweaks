using UnityEngine;

namespace ControllerTweaks.Interfaces
{
    public interface ISettingsSubviewController
    {
        void Activate(RectTransform parentTransform);
        void Deactivate();
        void ApplyChanges();
    }
}
