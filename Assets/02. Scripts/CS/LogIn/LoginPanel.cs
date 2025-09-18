using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;

public class LoginPanel : MonoBehaviour
{
    [Header("Root & Widgets")]
    [SerializeField] GameObject root;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Toggle rememberMeToggle;
    [SerializeField] Button submitButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button togglePwButton;
    [SerializeField] TMP_Text errorText;

    [Header("Events")]
    public UnityEvent<string, string, bool> onSubmit;
    public UnityEvent onCancel;

    const string LAST_EMAIL_KEY = "LOGIN_LAST_EMAIL";
    bool pwVisible = false;

    GameObject Root => root ? root : gameObject;

    void Reset() { root = gameObject; }
#if UNITY_EDITOR
    void OnValidate() { if (!root) root = gameObject; }
#endif

    void Awake()
    {
        if (submitButton) { submitButton.onClick.RemoveAllListeners(); submitButton.onClick.AddListener(TrySubmit); }
        if (cancelButton) { cancelButton.onClick.RemoveAllListeners(); cancelButton.onClick.AddListener(CancelAndClose); }
        if (togglePwButton) { togglePwButton.onClick.RemoveAllListeners(); togglePwButton.onClick.AddListener(TogglePasswordVisible); }
        if (errorText) errorText.text = "";

        // 처음엔 닫힌 상태
        HideInstant();
    }

    void OnEnable()
    {
        // 열릴 때 기본 포커스 & 에러 비우기
        if (errorText) errorText.text = "";
        if (emailInput) emailInput.ActivateInputField();
    }

    public void Show()
    {
        Root.SetActive(true);

        // rememberMe가 체크되어 있으면 마지막 이메일 복원, 아니면 비움
        if (rememberMeToggle && rememberMeToggle.isOn && PlayerPrefs.HasKey(LAST_EMAIL_KEY))
            emailInput.text = PlayerPrefs.GetString(LAST_EMAIL_KEY, "");
        else
            emailInput.text = "";

        // 비밀번호/표시상태 항상 초기화
        ResetPasswordField();

        // 포커스
        if (emailInput) emailInput.ActivateInputField();
    }

    public void Hide()
    {
        // 닫을 때 깔끔히 초기화
        Clear(keepRememberedEmail: true);
        Root.SetActive(false);
    }

    public void HideInstant() => Root.SetActive(false);

    // === Helpers ===

    void CancelAndClose()
    {
        // 취소 시에도 입력 초기화 후 닫기
        Clear(keepRememberedEmail: true);
        Root.SetActive(false);
        onCancel?.Invoke();
    }

    /// <summary>
    /// 입력칸/에러/비번표시 상태 초기화.
    /// keepRememberedEmail=true면 rememberMe 체크+PlayerPrefs 저장분은 남김.
    /// </summary>
    public void Clear(bool keepRememberedEmail)
    {
        // 이메일
        if (emailInput)
        {
            if (keepRememberedEmail && rememberMeToggle && rememberMeToggle.isOn && PlayerPrefs.HasKey(LAST_EMAIL_KEY))
                emailInput.text = PlayerPrefs.GetString(LAST_EMAIL_KEY, "");
            else
                emailInput.text = "";
        }

        // 비밀번호
        ResetPasswordField();

        // 에러 메시지
        if (errorText) errorText.text = "";

        // 선택/캐럿 정리
        if (emailInput) { emailInput.caretPosition = 0; emailInput.selectionStringAnchorPosition = 0; emailInput.selectionStringFocusPosition = 0; emailInput.DeactivateInputField(); }
        if (passwordInput) { passwordInput.caretPosition = 0; passwordInput.selectionStringAnchorPosition = 0; passwordInput.selectionStringFocusPosition = 0; passwordInput.DeactivateInputField(); }
    }

    void ResetPasswordField()
    {
        if (!passwordInput) return;
        pwVisible = false;
        passwordInput.text = "";
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }

    void TogglePasswordVisible()
    {
        pwVisible = !pwVisible;
        if (!passwordInput) return;
        passwordInput.contentType = pwVisible ? TMP_InputField.ContentType.Standard
                                              : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }

    void TrySubmit()
    {
        var email = emailInput ? emailInput.text.Trim() : "";
        var pw = passwordInput ? passwordInput.text : "";

        if (!IsValidEmail(email)) { SetError("이메일 형식이 올바르지 않습니다."); return; }
        if (string.IsNullOrEmpty(pw) || pw.Length < 6) { SetError("비밀번호를 6자 이상 입력하세요."); return; }

        // remember me 저장/삭제
        if (rememberMeToggle && rememberMeToggle.isOn) PlayerPrefs.SetString(LAST_EMAIL_KEY, email);
        else PlayerPrefs.DeleteKey(LAST_EMAIL_KEY);
        PlayerPrefs.Save();

        onSubmit?.Invoke(email, pw, rememberMeToggle && rememberMeToggle.isOn);
    }

    bool IsValidEmail(string s) => Regex.IsMatch(s, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    void SetError(string msg) { if (errorText) errorText.text = msg; }
}

