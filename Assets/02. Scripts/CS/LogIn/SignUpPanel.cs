using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;

public class SignUpPanel : MonoBehaviour
{
    [Header("Root & Widgets")]
    [SerializeField] GameObject root;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField confirmInput;
    [SerializeField] TMP_InputField nicknameInput;   // ★ 닉네임 추가
    [SerializeField] Button submitButton;   // 가입
    [SerializeField] Button cancelButton;   // 취소
    [SerializeField] Button togglePwButton;        // (선택) 비번 눈모양
    [SerializeField] Button toggleConfirmPwButton; // (선택) 확인 비번 눈모양
    [SerializeField] TMP_Text errorText;

    [Header("Events")]
    // 실제 회원가입 처리는 외부(네트워크 매니저 등)에서 구현하세요.
    // email, password 전달
    public UnityEvent<string, string> onSubmit;
    public UnityEvent onCancel;

    bool pwVisible = false;
    bool confirmVisible = false;

    GameObject Root => root ? root : gameObject;

    void Reset() { root = gameObject; }
#if UNITY_EDITOR
    void OnValidate() { if (!root) root = gameObject; }
#endif

    void Awake()
    {
        if (submitButton) { submitButton.onClick.RemoveAllListeners(); submitButton.onClick.AddListener(TrySubmit); }
        if (cancelButton) { cancelButton.onClick.RemoveAllListeners(); cancelButton.onClick.AddListener(CancelAndClose); }
        if (togglePwButton) { togglePwButton.onClick.RemoveAllListeners(); togglePwButton.onClick.AddListener(TogglePw); }
        if (toggleConfirmPwButton) { toggleConfirmPwButton.onClick.RemoveAllListeners(); toggleConfirmPwButton.onClick.AddListener(ToggleConfirmPw); }
        if (errorText) errorText.text = "";
        HideInstant();
    }

    public void Show()
    {
        Root.SetActive(true);
        Clear(); // 열릴 때 항상 초기화
        if (emailInput) emailInput.ActivateInputField();
    }

    public void Hide()
    {
        Clear();
        Root.SetActive(false);
    }

    public void HideInstant() => Root.SetActive(false);

    void Clear()
    {
        if (emailInput) emailInput.text = "";
        if (passwordInput) passwordInput.text = "";
        if (confirmInput) confirmInput.text = "";
        if (nicknameInput) nicknameInput.text = "";   // ★ 닉네임 비우기

        pwVisible = confirmVisible = false;

        if (passwordInput)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
            passwordInput.DeactivateInputField();
        }
        if (confirmInput)
        {
            confirmInput.contentType = TMP_InputField.ContentType.Password;
            confirmInput.ForceLabelUpdate();
            confirmInput.DeactivateInputField();
        }
        if (nicknameInput) // ★ 선택: 커서/포커스 제거
        {
            nicknameInput.contentType = TMP_InputField.ContentType.Standard;
            nicknameInput.ForceLabelUpdate();
            nicknameInput.DeactivateInputField();
        }

        if (errorText) errorText.text = "";
    }

    void CancelAndClose()
    {
        Hide();
        onCancel?.Invoke();
    }

    void TogglePw()
    {
        pwVisible = !pwVisible;
        if (!passwordInput) return;
        passwordInput.contentType = pwVisible ? TMP_InputField.ContentType.Standard
                                              : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }

    void ToggleConfirmPw()
    {
        confirmVisible = !confirmVisible;
        if (!confirmInput) return;
        confirmInput.contentType = confirmVisible ? TMP_InputField.ContentType.Standard
                                                  : TMP_InputField.ContentType.Password;
        confirmInput.ForceLabelUpdate();
    }

    void TrySubmit()
    {
        var email = emailInput ? emailInput.text.Trim() : "";
        var pw = passwordInput ? passwordInput.text : "";
        var cf = confirmInput ? confirmInput.text : "";

        if (!IsValidEmail(email)) { SetError("이메일 형식이 올바르지 않습니다."); return; }
        if (string.IsNullOrEmpty(pw) || pw.Length < 6) { SetError("비밀번호를 6자 이상 입력하세요."); return; }
        if (pw != cf) { SetError("비밀번호와 확인이 일치하지 않습니다."); return; }

        // 필요하면 추가 규칙(영문/숫자 조합 등) 여기에서 체크
        onSubmit?.Invoke(email, pw);
    }

    bool IsValidEmail(string s) => Regex.IsMatch(s, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    void SetError(string msg) { if (errorText) errorText.text = msg; }
}


