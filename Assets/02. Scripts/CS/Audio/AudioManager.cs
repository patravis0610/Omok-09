using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] string bgmParam = "BGMVolume";
    [SerializeField] string sfxParam = "SFXVolume";

    [Header("Sources")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxTemplate;   // Output=SFX, Clip 없음, PlayOnAwake Off
    [SerializeField] int sfxPoolSize = 6;

    List<AudioSource> sfxPool = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // SFX 풀 구성
        sfxPool.Add(sfxTemplate);
        for (int i = 1; i < sfxPoolSize; i++)
        {
            var src = Instantiate(sfxTemplate, transform);
            src.name = $"SFX {i}";
            sfxPool.Add(src);
        }

        // 저장값 적용
        SetBgm01(PlayerPrefs.GetFloat("BGM", 0.75f), save: false);
        SetSfx01(PlayerPrefs.GetFloat("SFX", 0.75f), save: false);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!clip) return;
        bgmSource.loop = loop;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSfx();
        src.pitch = pitch;
        src.spatialBlend = 0f;  // UI/2D 기본
        src.PlayOneShot(clip);
    }

    public void PlaySFXAt(AudioClip clip, Vector3 pos, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSfx();
        src.transform.position = pos;
        src.pitch = pitch;
        src.spatialBlend = 1f;  // 3D 사운드
        src.PlayOneShot(clip);
    }

    AudioSource GetFreeSfx()
    {
        foreach (var s in sfxPool) if (!s.isPlaying) return s;
        return sfxPool[0]; // 모두 바쁠 때 대체
    }

    // 0~1 슬라이더 값으로 설정
    public void SetBgm01(float v, bool save = true)
    {
        mixer.SetFloat(bgmParam, LinearToDb(v));
        if (save) PlayerPrefs.SetFloat("BGM", v);
    }
    public void SetSfx01(float v, bool save = true)
    {
        mixer.SetFloat(sfxParam, LinearToDb(v));
        if (save) PlayerPrefs.SetFloat("SFX", v);
    }

    float LinearToDb(float v) => (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
}

