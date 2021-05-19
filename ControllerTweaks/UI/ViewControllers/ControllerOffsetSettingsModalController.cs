using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using ControllerTweaks.Configuration;
using HMUI;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ControllerTweaks.UI
{
    public class ControllerOffsetSettingsModalController : IInitializable, INotifyPropertyChanged
    {
        private bool parsed = false;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        [UIComponent("modal")]
        private ModalView modalView;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform;

        private Vector3 modalPosition;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        internal event Action screenPositionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
            parsed = false;
        }

        internal void OnActivate()
        {
            if (parsed && modalTransform != null)
            {
                modalTransform.gameObject.SetActive(false);
            }
        }

        internal void OnDeactivate()
        {
            if (parsed && rootTransform != null && modalTransform != null)
            {
                modalTransform.SetParent(rootTransform);
            }
        }

        private void Parse(RectTransform parent)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "ControllerTweaks.UI.Views.ControllerOffsetSettingsModal.bsml"), parent.gameObject, this);
                modalPosition = modalTransform.localPosition;
                parsed = true;
            }
            modalTransform.SetParent(parent);
            modalTransform.localPosition = modalPosition;
            FieldAccessor<ModalView, bool>.Set(ref modalView, "_animateParentCanvas", true);
        }

        internal void ShowModal(RectTransform parent)
        {
            Parse(parent);
            parserParams.EmitEvent("close-modal");
            parserParams.EmitEvent("open-modal");
        }

        [UIAction("position-formatter")]
        private string PositionFormatter(int index)
        {
            return ((ControllerOffsetPauseViewController.ScreenPosition)index).ToString();
        }

        [UIValue("offset-delay-enabled")]
        private bool OffsetApplyDelayEnabled
        {
            get => PluginConfig.Instance.OffsetApplyDelayEnabled;
            set
            {
                PluginConfig.Instance.OffsetApplyDelayEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OffsetApplyDelayEnabled)));
            }
        }

        [UIValue("screen-position")]
        private int ScreenPosition
        {
            get => (int)PluginConfig.Instance.ScreenPosition;
            set
            {
                PluginConfig.Instance.ScreenPosition = (ControllerOffsetPauseViewController.ScreenPosition)value;
                screenPositionChanged?.Invoke();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScreenPosition)));
            }
        }

    }
}
