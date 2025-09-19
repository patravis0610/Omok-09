using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public BoardController boardController;         // board을 처리할 객체

    private Constants.PlayerType[,] _board;         // 보드의 상태 정보

    public BasePlayerState firstPlayerState;        // Player A
    public BasePlayerState secondPlayerState;       // Player B

    private BasePlayerState _currentPlayerState;    // 현재 턴의 Player

    public enum GameResult { None, Win, Lose, Draw }

    public GameLogic(BoardController boardController, Constants.GameType gameType)
    {
        this.boardController = boardController;

        // 보드의 상태 정보 초기화
        _board = new Constants.PlayerType[Constants.BlockColumnCount, Constants.BlockColumnCount];

        switch (gameType)
        {
            case Constants.GameType.SinglePlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new AIState();
                SetState(firstPlayerState);
                break;
            case Constants.GameType.DualPlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new PlayerState(false);
                // 게임 시작
                SetState(firstPlayerState);
                break;
            case Constants.GameType.MultiPlay:
                break;
        }

    }

    public Constants.PlayerType[,] GetBoard()
    {
        return _board;
    }


    // 턴이 바뀔 때, 기존 진행하던 상태를 Exit 하고
    // 이번 턴의 상태를 _currentPlayerState에 할당하고
    // 이번 턴의 상탱에 Enter 호출
    public void SetState(BasePlayerState state)
    {
        _currentPlayerState?.OnExit(this);
        _currentPlayerState = state;
        _currentPlayerState?.OnEnter(this);
    }
   
    public bool SetNewBoardValue(Constants.PlayerType playerType, int row, int col)
    {
        if (_board[row, col] != Constants.PlayerType.None) return false;

        if (playerType == Constants.PlayerType.Player_Black)
        {
            _board[row, col] = playerType;
            boardController.PlaceMaker(Block.MarkerType.Black, row, col);

            return true;
        }
        else if (playerType == Constants.PlayerType.Player_White)
        {
            _board[row, col] = playerType;
            boardController.PlaceMaker(Block.MarkerType.White, row, col);
            return true;
        }
        
        return false;
    }


    // Game Over 처리
    public void EndGame(GameResult gameResult)
    {
        SetState(null);
        firstPlayerState = null;
        secondPlayerState = null;

        if (gameResult == GameResult.Win)
        {
            Debug.Log("Win");
            GameManager.Instance.OpenConfirmPanel("Win!", () =>
            {
                //  GameManager.Instance.ChangeToMainScene();
            });
        }
        else if (gameResult == GameResult.Lose)
        {
            Debug.Log("Lose");
            // 유저에게 Game Over 표시
            GameManager.Instance.OpenConfirmPanel("Lose!", () =>
            {
                //  GameManager.Instance.ChangeToMainScene();
            });
        }
    }

    public GameResult CheckGameResult()
    {
        if (OmokAI.CheckGameWin(Constants.PlayerType.Player_Black, _board)) { return GameResult.Win; }
        if (OmokAI.CheckGameWin(Constants.PlayerType.Player_White, _board)) { return GameResult.Lose; }
        //if (OmokAI.CheckGameDraw(_board)) { return GameResult.Draw; }
        return GameResult.None;
    }

}
