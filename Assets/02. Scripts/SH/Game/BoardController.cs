using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;

    private Block _lastPlacedBlock;

    public delegate void OnBlockClicked(int row, int col);
    public OnBlockClicked OnBlockClickedDelegate;

    // 1. 모든 Block을 초기화
    public void InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                // 특정 Block이 클릭 된 상태에 대한 처리
                var row = blockIndex / Constants.BlockColumnCount;
                var col = blockIndex % Constants.BlockColumnCount;
                OnBlockClickedDelegate?.Invoke(row, col);
            });
        }
    }

    public void PlaceMaker(Block.MarkerType markerType, int row, int col)
    {
        // row, col >> index 변환
        var blockIndex = row * Constants.BlockColumnCount + col;
        blocks[blockIndex].SetMarker(markerType);

        // 이전 마지막 돌 꺼주기
        if (_lastPlacedBlock != null)
            _lastPlacedBlock.SetLastPosition(false);

        // 이번 돌을 마지막으로 표시
        blocks[blockIndex].SetLastPosition(true);
        _lastPlacedBlock = blocks[blockIndex];
    }

    public void UpdateForbiddenMarkers(Constants.PlayerType[,] board)
    {
        if (board == null)
        {
            foreach (var block in blocks)
                block.SetForbiddenMarker(false);
            return;
        }

        int size = Constants.BlockColumnCount;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                var blockIndex = row * Constants.BlockColumnCount + col;

                // 흑돌 금수 여부 판정
                bool forbidden = RenjuRule.IsForbiddenMove(board, row, col);

                blocks[blockIndex].SetForbiddenMarker(forbidden);
            }
        }
    }
}
