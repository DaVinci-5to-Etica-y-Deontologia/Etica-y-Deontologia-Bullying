using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSliderManager : MonoBehaviour
{
    [SerializeField] List<Mixers> _Mixers = new();
    void Start()
    {
        foreach (var item in _Mixers)
        {
            item.GetValue();
            item.SetUp();
            RefreshSlider(item.Slider, item.Value);
        }
    }

    void Update()
    {
        foreach (var item in _Mixers)
            SetVolumeFromSlider(item);
    }

    public void ResumeGame()
    {
        //GameManager.Instance.ClosePauseMenu();
    }

    public void SetVolume(Mixers mixer)
    {

        RefreshSlider(mixer.Slider, mixer.Value);
        mixer.AudioMixer.SetFloat(mixer.ExposedParamaterName, Mathf.Log10(mixer.Value)* 20f);
        mixer.SaveValue();
    }

    public void RefreshSlider(Slider slider, float value)
    {
        slider.value = value;
    }
    public void SetVolumeFromSlider(Mixers mixer)
    {
        SetVolume(mixer);
    }



    [System.Serializable]
    public class Mixers
    {
        public AudioMixer AudioMixer;
        public Slider Slider;
        public string ExposedParamaterName;
        public float Value;

        public void SaveValue()
        {
            PlayerPrefs.SetFloat(ExposedParamaterName, Slider.value);
        }
        public void GetValue()
        {
            Value = PlayerPrefs.GetFloat(ExposedParamaterName, Slider.value);
            Slider.value = Value;
        }

        public void SetUp()
        {
            Slider.onValueChanged.AddListener(GetValueFromSlider);
        }

        public void GetValueFromSlider(float value)
        {
            Value = value;
        }

       
    }
}
