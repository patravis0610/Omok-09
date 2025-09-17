// AudioOptionsUI.cs  (옵션 씬의 Sound Popup 같은 오브젝트에)
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    const string KEY_BGM = "vol.bgm";
    const string KEY_SFX = "vol.sfx";

    [SerializeField] AudioMixer mixer;      // Master
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    bool _init;

    void Awake()
    {
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    void OnEnable()
    {
        _init = true;
        float bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.75f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);
        bgmSlider.SetValueWithoutNotify(bgm);
        sfxSlider.SetValueWithoutNotify(sfx);
        mixer.SetFloat("BGMVolume", ToDb(bgm));
        mixer.SetFloat("SFXVolume", ToDb(sfx));
        _init = false;
    }

    void OnBgmChanged(float v) { if (_init) return; mixer.SetFloat("BGMVolume", ToDb(v)); PlayerPrefs.SetFloat(KEY_BGM, v); PlayerPrefs.Save(); }
    void OnSfxChanged(float v) { if (_init) return; mixer.SetFloat("SFXVolume", ToDb(v)); PlayerPrefs.SetFloat(KEY_SFX, v); PlayerPrefs.Save(); }

    float ToDb(float v) => (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
}



