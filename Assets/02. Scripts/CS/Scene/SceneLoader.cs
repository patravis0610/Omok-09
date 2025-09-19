using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Names (Build Settings에 등록된 이름)")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string optionsSceneName = "Options";

    public void LoadOptions() => LoadSceneSafe(optionsSceneName);
    public void LoadMainMenu() => LoadSceneSafe(mainMenuSceneName);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadSceneSafe(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is empty on SceneLoader. 인스펙터에서 이름을 설정하세요.");
            return;
        }

        // Build Settings에 등록되어 있고 로드 가능한지 검사
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return;
        }

        // 등록된 씬 목록 출력해 원인 파악 도움
        int count = SceneManager.sceneCountInBuildSettings;
        string list = "";
        for (int i = 0; i < count; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var name = Path.GetFileNameWithoutExtension(path);
            list += (i == 0 ? "" : ", ") + name;
        }

        Debug.LogError(
            $"Scene '{sceneName}' 를 로드할 수 없습니다. " +
            $"Build Settings에 등록되어 있는지, 이름이 정확한지 확인하세요.\n" +
            $"현재 등록된 씬: [{list}]");
    }
}

