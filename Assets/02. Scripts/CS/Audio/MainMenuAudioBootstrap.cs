// MainMenuAudioBootstrap.cs (스샷처럼 붙여두면 OK)
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuAudioBootstrap : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource bgm;

    void Awake()
    {
        float v = PlayerPrefs.GetFloat("vol.bgm", 0.75f);
        mixer.SetFloat("BGMVolume", (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f);
    }
    void Start()
    {
        if (!bgm) bgm = GetComponent<AudioSource>();
        if (bgm && !bgm.isPlaying) bgm.Play();
    }
}





