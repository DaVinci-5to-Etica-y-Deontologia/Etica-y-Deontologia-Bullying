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

        PlayerPrefs.SetFloat(transform.gameObject.name + "Mixer", volume);
        PlayerPrefs.SetFloat(transform.gameObject.name + "Slider", _slider.value);

        _master.SetFloat(name, volume);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(transform.gameObject.name + "Slider"))
        {
            _slider.value = PlayerPrefs.GetFloat(transform.gameObject.name + "Slider");
        }
    }
}
