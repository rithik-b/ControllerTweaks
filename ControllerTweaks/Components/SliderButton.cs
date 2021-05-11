using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ControllerTweaks.Components
{
    public class SliderButton : MonoBehaviour
    {
        private Button button;
        private SliderSetting slider;
        private float step;

        public static void Register(RectTransform leftButton, RectTransform rightButton, SliderSetting slider, float step)
        {
            leftButton.gameObject.SetActive(true);
            leftButton.SetParent(slider.transform.Find("BSMLSlider"));
            leftButton.localPosition = new Vector3(-41, -1.03f, 0);
            leftButton.localScale = new Vector3(0.5f, 1, 1);
            SliderButton sliderButton = leftButton.gameObject.AddComponent<SliderButton>();
            sliderButton.Setup(slider, -step);

            rightButton.gameObject.SetActive(true);
            rightButton.SetParent(slider.transform.Find("BSMLSlider"));
            rightButton.localPosition = new Vector3(1, -1.03f, 0);
            rightButton.localScale = new Vector3(0.5f, 1, 1);
            sliderButton = rightButton.gameObject.AddComponent<SliderButton>();
            sliderButton.Setup(slider, step);
        }

        public void Setup(SliderSetting slider, float step)
        {
            this.slider = slider;
            this.step = step;
        }

        public virtual void OnEnable()
        {
            button = this.gameObject.GetComponent<NoTransitionsButton>();
            button?.onClick.AddListener(Step);
        }

        public void OnDisable()
        {
            button?.onClick.RemoveAllListeners();
        }

        private void Step()
        {
            if (slider != null)
            {
                slider.associatedValue.SetValue((float)slider.associatedValue.GetValue() + step);
            }
        }

    }
}
