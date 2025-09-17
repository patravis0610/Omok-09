using UnityEngine;
using UnityEngine.Audio;

public class AudioBootstrap : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string bgmParam = "BGMVolume";
    [SerializeField] string sfxParam = "SFXVolume";

    void Awake()
    {
        float bgm = PlayerPrefs.GetFloat("BGM", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFX", 0.75f);
        mixer.SetFloat(bgmParam, LinearToDb(bgm));
        mixer.SetFloat(sfxParam, LinearToDb(sfx));
    }

    float LinearToDb(float v) => (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
}

