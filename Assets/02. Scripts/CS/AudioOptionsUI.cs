using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    [Header("Mixer & Params")]
    public AudioMixer mixer;           // 만든 AudioMixer 에셋
    public string sfxParam = "SFXVolume";
    public string bgmParam = "BGMVolume";
    [SerializeField] private float minDb = -80f; // 사실상 음소거
    [SerializeField] private float maxDb = 0f;   // 일반적으로 0dB가 최대

    [Header("UI Sliders (0~1)")]
    public Slider sfxSlider;           // 효과음 슬라이더
    public Slider bgmSlider;           // 배경음 슬라이더

    private void Awake()
    {
        // 저장값 불러오기 (기본 0.75)
        float sfx = PlayerPrefs.GetFloat(sfxParam, 0.75f);
        float bgm = PlayerPrefs.GetFloat(bgmParam, 0.75f);

        if (sfxSlider) sfxSlider.SetValueWithoutNotify(Mathf.Clamp01(sfx));
        if (bgmSlider) bgmSlider.SetValueWithoutNotify(Mathf.Clamp01(bgm));

        ApplyDb(sfxParam, sfx);
        ApplyDb(bgmParam, bgm);

        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (bgmSlider) bgmSlider.onValueChanged.AddListener(OnBgmChanged);
    }

    private void OnDestroy()
    {
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        if (bgmSlider) bgmSlider.onValueChanged.RemoveListener(OnBgmChanged);
    }

    private void OnSfxChanged(float v)
    {
        ApplyDb(sfxParam, v);
        PlayerPrefs.SetFloat(sfxParam, Mathf.Clamp01(v));
        PlayerPrefs.Save();
    }

    private void OnBgmChanged(float v)
    {
        ApplyDb(bgmParam, v);
        PlayerPrefs.SetFloat(bgmParam, Mathf.Clamp01(v));
        PlayerPrefs.Save();
    }

    private void ApplyDb(string param, float linear01)
    {
        float dB = (linear01 <= 0.0001f) ? minDb : Mathf.Clamp(Mathf.Log10(linear01) * 20f, minDb, maxDb);
        mixer.SetFloat(param, dB);
    }
}
