using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-1000)] // 제일 먼저 실행
public class AudioInit : MonoBehaviour
{
    public AudioMixer mixer;                 // Master 믹서 에셋
    public string bgmParam = "BGMVolume";    // Exposed 이름과 동일
    public string sfxParam = "SFXVolume";
    [SerializeField] private float minDb = -80f;

    void Awake()
    {
        // 여러 씬을 넘어가도 유지되게 하고 싶으면
        DontDestroyOnLoad(gameObject);

        ApplySaved(bgmParam);
        ApplySaved(sfxParam);
    }

    void ApplySaved(string param)
    {
        float v = PlayerPrefs.GetFloat(param, 0.75f); // 0~1 값
        float dB = (v <= 0.0001f) ? minDb : Mathf.Log10(v) * 20f;
        mixer.SetFloat(param, Mathf.Clamp(dB, minDb, 0f));
    }
}
