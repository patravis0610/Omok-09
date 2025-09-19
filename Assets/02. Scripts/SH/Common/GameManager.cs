using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject confirmPanel;           // 확인 패널

    // Panel을 띄우기 위한 Canvas 정보
    private Canvas _canvas;

    // Main Scene에서 선택한 게임 타입
    private Constants.GameType _gameType;

    // Game 씬의 UI를 담당하는 객체
    private GameUIController _gameUIController;

    // Game Logic
    private GameLogic _gameLogic;


    public void OpenConfirmPanel(string message,ConfirmPanelController.OnConfirmButtonClicked onConfirmButtonClicked)
    {
        if (_canvas != null)
        {
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, onConfirmButtonClicked);
        }
    }
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _canvas = FindFirstObjectByType<Canvas>();

        if (scene.name == "GameScene_SH")
        {
            // Block 초기화
            var blockController = FindFirstObjectByType<BoardController>();
            if (blockController != null)
            {
                blockController.InitBlocks();
            }

            // Game UI Controller 할당 및 초기화
            _gameUIController = FindFirstObjectByType<GameUIController>();
            if (_gameUIController != null)
            {
                _gameUIController.SetGameTurnPanel(GameUIController.GameTurnPanelType.None);
            }
            _gameUIController.SetGameTitle(_gameType);

            // GameLogic 생성
            //if (_gameLogic != null) _gameLogic.Dispose();
            _gameLogic = new GameLogic(blockController, _gameType);
        }
    }

    /// <summary>
    /// Game Scene에서 턴을 표시하는 UI를 제어하는 함수
    /// </summary>
    /// <param name="gameTurnPanelType">표시할 Turn 정보</param>
    public void SetGameTurnPanel(GameUIController.GameTurnPanelType gameTurnPanelType)
    {
        _gameUIController.SetGameTurnPanel(gameTurnPanelType);
    }


    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("GameScene_SH");
    }

    /// <summary>
    /// Game에서 Main Scene으로 전환시 호출될 메서드
    /// </summary>
    public void ChangeToMainScene()
    {
        //_gameLogic?.Dispose();
        _gameLogic = null;
        SceneManager.LoadScene("MainScene_Test");
    }
}
