using UnityEngine;

public class QuitConfirmUI : MonoBehaviour
{
    [SerializeField] private GameObject panel; // QuitConfirmPanel(비활성화)

    public void Open() => panel.SetActive(true);   // 종료 버튼에서 호출
    public void Close() => panel.SetActive(false);  // 아니오 버튼에서 호출

    public void ConfirmQuit()                      // 예 버튼에서 호출
    {
        Close();
        PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Debug.Log("WebGL은 종료 불가: 브라우저 탭을 닫으세요.");
#else
        Application.Quit();
#endif
    }
}


