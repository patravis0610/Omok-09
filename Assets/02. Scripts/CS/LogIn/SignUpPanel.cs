using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

public class SignUpPanel : MonoBehaviour
{
    [Header("Root & Widgets")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmInput;
    [SerializeField] private TMP_InputField nicknameInput;     // ¡Ú ´Ð³×ÀÓ
    [SerializeField] private Button submitButton;      // °¡ÀÔ
    [SerializeField] private Button cancelButton;      // Ãë¼Ò
    [SerializeField] private Button togglePwButton;    // (¼±ÅÃ) ºñ¹ø ´«¸ð¾ç
    [SerializeField] private Button toggleConfirmPwButton; // (¼±ÅÃ) È®ÀÎ ºñ¹ø ´«¸ð¾ç
    [SerializeField] private TMP_Text errorText;

    [Header("Events")]
    // ±âÁ¸ È£È¯: ÀÌ¸ÞÀÏ/ºñ¹ø¸¸ ¿ÜºÎ·Î ³Ñ±è (¼­¹ö ¿¬µ¿ µîÀº ¿ÜºÎ¿¡¼­)
    public UnityEvent<string, string> onSubmit;
    public UnityEvent onCancel;

    private bool pwVisible = false;
    private bool confirmVisible = false;

    private GameObject Root => root ? root : gameObject;

    void Reset() { root = gameObject; }
#if UNITY_EDITOR
    void OnValidate() { if (!root) root = gameObject; }
#endif

    void Awake()
    {
        if (submitButton)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(TrySubmit);
        }
        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelAndClose);
        }
        if (togglePwButton)
        {
            togglePwButton.onClick.RemoveAllListeners();
            togglePwButton.onClick.AddListener(TogglePw);
        }
        if (toggleConfirmPwButton)
        {
            toggleConfirmPwButton.onClick.RemoveAllListeners();
            toggleConfirmPwButton.onClick.AddListener(ToggleConfirmPw);
        }

        if (errorText) errorText.text = "";
        HideInstant(); // ½ÃÀÛ ½Ã ºñÈ°¼ºÈ­
    }

    // === ¿ÜºÎ¿¡¼­ ¿­±â/´Ý±â¿ë API (MainMenuUI µî¿¡¼­ È£Ãâ) ===
    public void Show()
    {
        Root.SetActive(true);
        Clear();                       // Ç×»ó ÃÊ±âÈ­
        StartCoroutine(FocusNickname()); // ´Ð³×ÀÓ ¸ÕÀú Æ÷Ä¿½º
    }

    public void Hide()
    {
        Clear();
        Root.SetActive(false);
    }

    public void HideInstant() => Root.SetActive(false);
    // ========================================================

    /// <summary> ¸ðµç ÀÔ·Â/Ç¥½Ã »óÅÂ ÃÊ±âÈ­ </summary>
    private void Clear()
    {
        if (emailInput) emailInput.text = "";
        if (passwordInput) passwordInput.text = "";
        if (confirmInput) confirmInput.text = "";
        if (nicknameInput) nicknameInput.text = ""; // ¡Ú ´Ð³×ÀÓ ºñ¿ì±â

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
        if (nicknameInput)
        {
            nicknameInput.contentType = TMP_InputField.ContentType.Standard;
            nicknameInput.ForceLabelUpdate();
            nicknameInput.DeactivateInputField();
        }

        if (errorText) errorText.text = "";
    }

    private System.Collections.IEnumerator FocusNickname()
    {
        // SetActive(true) Á÷ÈÄ ÇÑ ÇÁ·¹ÀÓ µÚ¿¡ Æ÷Ä¿½º ÁÖ´Â °Ô ¾ÈÁ¤Àû
        yield return null;

        if (nicknameInput && nicknameInput.interactable)
        {
            EventSystem.current?.SetSelectedGameObject(nicknameInput.gameObject);
            nicknameInput.ActivateInputField();
            nicknameInput.caretPosition = nicknameInput.text.Length;
        }
        else if (emailInput)
        {
            EventSystem.current?.SetSelectedGameObject(emailInput.gameObject);
            emailInput.ActivateInputField();
        }
    }

    private void CancelAndClose()
    {
        Hide();
        onCancel?.Invoke();
    }

    private void TogglePw()
    {
        pwVisible = !pwVisible;
        if (!passwordInput) return;

        passwordInput.contentType = pwVisible
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }

    private void ToggleConfirmPw()
    {
        confirmVisible = !confirmVisible;
        if (!confirmInput) return;

        confirmInput.contentType = confirmVisible
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        confirmInput.ForceLabelUpdate();
    }

    private void TrySubmit()
    {
        string email = emailInput ? emailInput.text.Trim() : "";
        string pw = passwordInput ? passwordInput.text : "";
        string cf = confirmInput ? confirmInput.text : "";
        string nick = nicknameInput ? nicknameInput.text.Trim() : "";

        if (!IsValidEmail(email)) { SetError("ÀÌ¸ÞÀÏ Çü½ÄÀÌ ¿Ã¹Ù¸£Áö ¾Ê½À´Ï´Ù."); return; }
        if (string.IsNullOrEmpty(pw) || pw.Length < 6) { SetError("ºñ¹Ð¹øÈ£¸¦ 6ÀÚ ÀÌ»ó ÀÔ·ÂÇÏ¼¼¿ä."); return; }
        if (pw != cf) { SetError("ºñ¹Ð¹øÈ£¿Í È®ÀÎÀÌ ÀÏÄ¡ÇÏÁö ¾Ê½À´Ï´Ù."); return; }
        if (!IsValidNickname(nick)) { SetError("´Ð³×ÀÓÀº 2~16ÀÚ, ÇÑ±Û/¿µ¹®/¼ýÀÚ/_ ¸¸ °¡´ÉÇÕ´Ï´Ù."); return; }

        // ±âÁ¸ È£È¯: ÀÌ¸ÞÀÏ/ºñ¹ø¸¸ Àü´Þ (´Ð³×ÀÓÀº ¿ÜºÎ¿¡¼­ nicknameInput.text·Î Á÷Á¢ ÂüÁ¶ÇØµµ µÊ)
        onSubmit?.Invoke(email, pw);
    }

    // ===== À¯Æ¿ =====
    private bool IsValidEmail(string s)
        => Regex.IsMatch(s, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");

    // ÇÑ±Û/¿µ¹®/¼ýÀÚ/¹ØÁÙ 2~16ÀÚ, °ø¹é ±ÝÁö
    private bool IsValidNickname(string s)
        => !string.IsNullOrWhiteSpace(s) &&
           Regex.IsMatch(s, @"^[°¡-ÆRa-zA-Z0-9_]{2,16}$");

    private void SetError(string msg)
    {
        if (errorText) errorText.text = msg;
        Debug.LogWarning(msg);
    }
}
