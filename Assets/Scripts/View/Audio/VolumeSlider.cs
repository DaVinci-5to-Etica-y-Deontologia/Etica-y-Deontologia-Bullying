using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private AudioMixer _master;

    public void VolumeController(string name)
    {
        float volume = 20 * Mathf.Log10(_slider.value);
        volume = _slider.value == 0 ? -80 : volume;

        _master.SetFloat(name, volume);
    }
}
