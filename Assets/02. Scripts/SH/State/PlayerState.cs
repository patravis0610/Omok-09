using UnityEngine;

public class PlayerState : BasePlayerState
{
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;

    private bool _isMultiplay;




    public PlayerState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ?
            Constants.PlayerType.Player_Black : Constants.PlayerType.Player_White;
        _isMultiplay = false;
    }


    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        // 1. First Player인지 확인해서 게임 UI에 현재 턴 표시
        if (_isFirstPlayer)
        {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.Balck_Turn);
        }
        else
        {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.White_Turn);
        }

        if (_playerType == Constants.PlayerType.Player_Black)
        {
            gameLogic.boardController.UpdateForbiddenMarkers(gameLogic.GetBoard());
        }
        else
        {
            // 백돌 차례일 때는 금수 마커 숨기기
            gameLogic.boardController.UpdateForbiddenMarkers(null);
        }

        // 2. Block Controller에게 해야 할 일을 전달
        gameLogic.boardController.OnBlockClickedDelegate = (row, col) =>
        {
            // Block이 터치 될 때까지 기다렸다가 터치 되면 처리할 일
            HandleMove(gameLogic, row, col);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        gameLogic.boardController.OnBlockClickedDelegate = null;
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
        {
            Debug.Log("_isFirstPlayer");
            gameLogic.SetState(gameLogic.secondPlayerState);
        }
        else
        {
            Debug.Log("secondPlayerState");
            gameLogic.SetState(gameLogic.firstPlayerState);
        }
    }
}
