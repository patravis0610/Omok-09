using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer;
    [SerializeField] private string bgmParam = "BGMVolume";
    [SerializeField] private string sfxParam = "SFXVolume";
    [SerializeField] private float minDb = -80f;
    [SerializeField] private float maxDb = 0f;

    [Header("Optional: 같은 오브젝트에 슬라이더 연결 가능")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string BgmKey = "BGMVolume";
    private const string SfxKey = "SFXVolume";

    private void Awake()
    {
        // 루트 오브젝트 보장 + 싱글턴 + 유지
        if (transform.parent != null) transform.SetParent(null);
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioListener.volume = 1f;
        AudioListener.pause = false;

        // 저장값 즉시 1차 적용
        ApplyAllFromPrefs();

        // 슬라이더 동기화
        if (bgmSlider) { bgmSlider.SetValueWithoutNotify(GetBgm01()); bgmSlider.onValueChanged.AddListener(SetBgm); }
        if (sfxSlider) { sfxSlider.SetValueWithoutNotify(GetSfx01()); sfxSlider.onValueChanged.AddListener(SetSfx); }
    }

    private void OnEnable()
    {
        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigChanged;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigChanged;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void Start()
    {
        // ▶ 중요: 플레이 직후 오디오 초기화가 끝난 다음 프레임에도 다시 한번 적용
        StartCoroutine(ReapplyNextFrames());
    }

    private IEnumerator ReapplyNextFrames()
    {
        // 한 프레임 뒤 + EndOfFrame에 2번 재적용하면
        // 초기 스냅샷/오디오 초기화로 덮어씌워지는 문제를 안정적으로 잡을 수 있음
        yield return null;
        ApplyAllFromPrefs();

        yield return new WaitForEndOfFrame();
        ApplyAllFromPrefs();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (bgmSlider) bgmSlider.onValueChanged.RemoveListener(SetBgm);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(SetSfx);
    }

    // ---- 이벤트 훅 ----
    private void OnAudioConfigChanged(bool deviceWasChanged)
    {
        // 에디터에서 Play/Stop/오디오 디바이스 변경 등으로 오디오 엔진이 리셋될 때 재적용
        ApplyAllFromPrefs();
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // 씬 전환 시에도 재적용 (혹시 스냅샷/초기화 스크립트가 있으면 대비)
        ApplyAllFromPrefs();
    }

    // ---- Public API ----
    public void SetBgm(float v)
    {
        v = Mathf.Clamp01(v);
        ApplyDb(bgmParam, v);
        PlayerPrefs.SetFloat(BgmKey, v);
        PlayerPrefs.Save();
        LogMixerValue(bgmParam);
    }

    public void SetSfx(float v)
    {
        v = Mathf.Clamp01(v);
        ApplyDb(sfxParam, v);
        PlayerPrefs.SetFloat(SfxKey, v);
        PlayerPrefs.Save();
        LogMixerValue(sfxParam);
    }

    public float GetBgm01() => PlayerPrefs.GetFloat(BgmKey, 0.75f);
    public float GetSfx01() => PlayerPrefs.GetFloat(SfxKey, 0.75f);

    // ---- 내부 헬퍼 ----
    private void ApplyAllFromPrefs()
    {
        float bgm = GetBgm01();
        float sfx = GetSfx01();
        ApplyDb(bgmParam, bgm);
        ApplyDb(sfxParam, sfx);
        LogMixerValue(bgmParam);
        LogMixerValue(sfxParam);
    }

    private void ApplyDb(string param, float linear01)
    {
        float db = (linear01 <= 0.0001f) ? minDb : Mathf.Clamp(Mathf.Log10(linear01) * 20f, minDb, maxDb);
        mixer.SetFloat(param, db);
    }

    private void LogMixerValue(string param)
    {
        if (!mixer) { Debug.LogError("[Audio] Mixer 참조 없음"); return; }
        if (mixer.GetFloat(param, out float nowDb))
            Debug.Log($"[Audio] {param} = {nowDb:0.0} dB 적용됨");
        else
            Debug.LogError($"[Audio] '{param}' 파라미터를 찾지 못했습니다. (Expose 이름 오타 가능)");
    }
}


