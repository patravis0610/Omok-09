// AudioInitBeforeScene.cs
using UnityEngine;
using UnityEngine.Audio;

public static class AudioInitBeforeScene
{
    const string KEY_BGM = "vol.bgm";
    const string KEY_SFX = "vol.sfx";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ApplySavedVolumes()
    {
        var mixer = Resources.Load<AudioMixer>("Audio/Master");
        if (!mixer) { Debug.LogError("[AudioInit] Resources/Audio/Master.mixer not found"); return; }

        float bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.75f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);

        bool okB = mixer.SetFloat("BGMVolume", ToDb(bgm));
        bool okS = mixer.SetFloat("SFXVolume", ToDb(sfx));

        // 디버그 + 두 믹서가 같은지 확인용(아래 ③에서 비교)
        Debug.Log($"[AudioInit] BGM={bgm} ok={okB}, SFX={sfx} ok={okS}");
    }

    static float ToDb(float v) => (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
}






