using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetBGM(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    public void SetSFX(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }
}

