using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AuthPopup : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button closeButton;

    public UnityEvent onLoginClicked;
    public UnityEvent onSignUpClicked;

    private GameObject Root => root ? root : gameObject;

    private void Reset() { root = gameObject; }
#if UNITY_EDITOR
    private void OnValidate() { if (!root) root = gameObject; }
#endif

    // AuthPopup.cs (수정)
    private void Awake()
    {
        if (loginButton) loginButton.onClick.AddListener(() => onLoginClicked?.Invoke());
        if (signUpButton) signUpButton.onClick.AddListener(() => onSignUpClicked?.Invoke());
        if (closeButton) closeButton.onClick.AddListener(Hide);

        // HideInstant();  // ← 이 줄 삭제
    }


    public void Show()
    {
        Root.SetActive(true);

        // 맨 위로 올려 뒤에 깔리지 않게
        transform.SetAsLastSibling();

        // CanvasGroup을 쓴 경우 보정
        var cg = GetComponent<CanvasGroup>();
        if (cg) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }
    }

    public void Hide()
    {
        var cg = GetComponent<CanvasGroup>();
        if (cg) { cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false; }
        Root.SetActive(false);
    }

    public void HideInstant() => Root.SetActive(false);
}





