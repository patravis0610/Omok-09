using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AuthPopup : MonoBehaviour
{
    public static AuthPopup Instance { get; private set; }

    [Header("Wiring")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject root;        // AuthPopup (this) 지정
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button closeButton;     // 선택
    [SerializeField] private Button backdropButton;  // Backdrop에 있는 Button

    [SerializeField] private float fadeTime = 0.15f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!root) root = gameObject;

        // 시작은 숨김
        HideImmediate();

        // 버튼 이벤트
        if (loginButton) loginButton.onClick.AddListener(OnLoginClicked);
        if (signupButton) signupButton.onClick.AddListener(OnSignupClicked);
        if (closeButton) closeButton.onClick.AddListener(Close);
        if (backdropButton) backdropButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (root.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Open()
    {
        StopAllCoroutines();
        root.SetActive(true);
        StartCoroutine(Fade(0f, 1f, true));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1f, 0f, false));
    }

    private void HideImmediate()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        root.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, bool keepActive)
    {
        if (!canvasGroup) yield break;

        canvasGroup.alpha = from;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = to;

        if (!keepActive)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            root.SetActive(false);
        }
    }

    // 여기는 원하는 인증 흐름으로 바꿔 연결하세요.
    private void OnLoginClicked()
    {
        Debug.Log("[AuthPopup] 로그인 클릭");
        // 예: SceneManager.LoadScene("LoginScene");
        // 혹은 자체 로그인 패널 열기
    }

    private void OnSignupClicked()
    {
        Debug.Log("[AuthPopup] 회원가입 클릭");
        // 예: Application.OpenURL("https://your.site/signup");
        // 혹은 회원가입 패널/씬 열기
    }
}

