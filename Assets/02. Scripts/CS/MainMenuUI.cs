using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button multiPlayButton;
    [SerializeField] private Button quitButton;

    [Header("Popup/Panel Refs")]
    [SerializeField] private AuthPopup authPopupPrefab;  // ★ Project의 AuthPopup.prefab을 여기로 드랙
    [SerializeField] private Transform uiParent;         // ★ Canvas
    [SerializeField] private LoginPanel loginPanel;      // ★ Hierarchy의 LoginPanel (없으면 비워둬도 됨)
    [SerializeField] private SignUpPanel signUpPanel;    // ★ Hierarchy의 SignUpPanel (없으면 비워둬도 됨)

    private AuthPopup authPopupInstance;

    private void Awake()
    {
        if (multiPlayButton) multiPlayButton.onClick.AddListener(OpenAuthPopup);
        if (quitButton) quitButton.onClick.AddListener(Application.Quit);
    }

    private void OpenAuthPopup()
    {
        Debug.Log("[Multi] onClick fired");

        if (authPopupInstance == null)
        {
            if (authPopupPrefab == null)
            {
                Debug.LogError("[MainMenuUI] authPopupPrefab is NULL. " +
                               "Project의 AuthPopup.prefab을 MainMenuUI의 authPopupPrefab 슬롯에 연결하세요.");
                return;
            }

            var parent = uiParent != null ? uiParent : transform;
            authPopupInstance = Instantiate(authPopupPrefab, parent);

            // 팝업 버튼 이벤트 → 각 패널 열기 (패널이 없으면 무시)
            authPopupInstance.onLoginClicked.AddListener(() =>
            {
                authPopupInstance.Hide();
                if (loginPanel) loginPanel.Show();
            });

            authPopupInstance.onSignUpClicked.AddListener(() =>
            {
                authPopupInstance.Hide();
                if (signUpPanel) signUpPanel.Show();
            });
        }

        authPopupInstance.Show();

        // 패널에서 취소 눌렀을 때 팝업으로 복귀(패널이 있을 때만)
        if (loginPanel)
        {
            loginPanel.onCancel.RemoveAllListeners();
            loginPanel.onCancel.AddListener(() => authPopupInstance.Show());
        }
        if (signUpPanel)
        {
            signUpPanel.onCancel.RemoveAllListeners();
            signUpPanel.onCancel.AddListener(() => authPopupInstance.Show());
        }
    }
}



