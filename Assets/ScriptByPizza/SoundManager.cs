using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    // ---------- SFX Library ----------
    [System.Serializable]
    public class SFXData
    {
        public string name;      // เช่น "Dead", "Shoot"
        public AudioClip clip;
    }

    [Header("SFX Library")]
    public SFXData[] sfxLibrary;

    private Dictionary<string, AudioClip> sfxDict;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitSFX();
    }

    void InitSFX()
    {
        sfxDict = new Dictionary<string, AudioClip>();

        foreach (var sfx in sfxLibrary)
        {
            if (!string.IsNullOrEmpty(sfx.name) && sfx.clip != null)
            {
                sfxDict[sfx.name] = sfx.clip;
            }
        }
    }

    // ---------- BGM ----------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ---------- SFX ----------
    public void PlaySFX(string sfxName, float volume = 1f)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + sfxName);
        }
    }

    // ---------- Volume ----------
    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }
}
