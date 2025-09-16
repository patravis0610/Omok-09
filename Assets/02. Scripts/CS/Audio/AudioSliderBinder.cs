using UnityEngine;
using UnityEngine.UI;

public class AudioSliderBinder : MonoBehaviour
{
    public enum Target { BGM, SFX }
    public Target target;
    public Slider slider;

    private void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
        if (!slider || AudioSettingsManager.Instance == null) return;

        // 현재 저장값을 UI에 반영하고 리스너 연결
        if (target == Target.BGM)
        {
            slider.SetValueWithoutNotify(AudioSettingsManager.Instance.GetBgm01());
            slider.onValueChanged.AddListener(AudioSettingsManager.Instance.SetBgm);
        }
        else
        {
            slider.SetValueWithoutNotify(AudioSettingsManager.Instance.GetSfx01());
            slider.onValueChanged.AddListener(AudioSettingsManager.Instance.SetSfx);
        }
    }

    private void OnDestroy()
    {
        if (!slider || AudioSettingsManager.Instance == null) return;
        if (target == Target.BGM) slider.onValueChanged.RemoveListener(AudioSettingsManager.Instance.SetBgm);
        else slider.onValueChanged.RemoveListener(AudioSettingsManager.Instance.SetSfx);
    }
}

