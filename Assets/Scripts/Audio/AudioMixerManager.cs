using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private string[] _audioCategories;
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        string audioKey = "";
        float audioValue = 0f;

        for (int i = 0; i < _audioCategories.Length; i++)
        {
            audioKey = _audioCategories[i] + "Mixer";
            audioValue = PlayerPrefs.GetFloat(audioKey);

            if (PlayerPrefs.HasKey(audioKey))
            {
                _audioMixer.SetFloat(_audioCategories[i], audioValue);
            }
            else
            {
                PlayerPrefs.SetFloat(audioKey, audioValue);
                _audioMixer.SetFloat(_audioCategories[i], audioValue);
            }
        }
    }
}
