using System.Collections.Generic;
using UnityEngine;

public static class OmokAI
{
    private static readonly (int, int)[] directions =
     {
        (0, 1),   // 가로 →
        (1, 0),   // 세로 ↓
        (1, 1),   // 대각 ↘
        (1, -1),  // 대각 ↙
    };

    /// <summary>
    /// 가장 좋은 수 찾기
    /// </summary>
    public static (int row, int col)? GetBestMove(Constants.PlayerType[,] board)
    {
        int bestScore = int.MinValue;
        (int, int) bestMove = (-1, -1);

        List<(int, int)> candidates = GetCandidateMoves(board);

        foreach (var (row, col) in candidates)
        {
            // 공격 점수 (AI가 흰돌이라고 가정)
            board[row, col] = Constants.PlayerType.Player_White;
            int attackScore = EvaluateBoard(board, row, col, Constants.PlayerType.Player_White);

            // 수비 점수 (상대가 검은돌 두면 위험한지 확인)
            board[row, col] = Constants.PlayerType.Player_Black;
            int defenseScore = EvaluateBoard(board, row, col, Constants.PlayerType.Player_Black);

            // 원래대로 복원
            board[row, col] = Constants.PlayerType.None;

            // 최종 점수 (수비에 가중치 2배)
            int totalScore = attackScore + defenseScore * 2;

            // 랜덤 요소 추가 (동점일 때 매번 같은 자리 안 두게)
            totalScore += Random.Range(0, 5);

            if (totalScore > bestScore)
            {
                bestScore = totalScore;
                bestMove = (row, col);
            }
        }

        return bestMove == (-1, -1) ? null : bestMove;
    }

    /// <summary>
    /// 후보 좌표 뽑기 (돌 주변만 탐색)
    /// 보드판전체를 검색시작하며 돌이없으면 패스, 있으면 그주변 두칸 빈곳만 저장
    /// </summary>
    private static List<(int, int)> GetCandidateMoves(Constants.PlayerType[,] board)
    {
        int size = board.GetLength(0);
        HashSet<(int, int)> candidates = new HashSet<(int, int)>();
        //List랑 형태는 비슷하지만 왜 List를 쓰지 않았냐면 
        //1.List는 따로 중복제거를 해줘야함
        //2. Hash는 내부적으로 테이블을 사용하기 때문에 추가/검색 속도가 더빠름

        int radius = 2; // 돌 주변 2칸만 고려
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (board[r, c] != Constants.PlayerType.None)
                {
                    for (int dr = -radius; dr <= radius; dr++)
                    {
                        for (int dc = -radius; dc <= radius; dc++)
                        {
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < size && 
                                nc >= 0 && nc < size &&
                                board[nr, nc] == Constants.PlayerType.None)
                            {
                                candidates.Add((nr, nc));
                            }
                        }
                    }
                }
            }
        }

        return new List<(int, int)>(candidates);
    }

    /// <summary>
    /// 특정 위치에 둔 경우 점수 평가
    /// </summary>
    private static int EvaluateBoard(Constants.PlayerType[,] board, int row, int col, Constants.PlayerType player)
    {
        int score = 0;

        foreach (var (dx, dy) in directions)
        {
            int count = 1; // 방금 둔 돌 포함
            int openEnds = 0;

            // 정방향
            count += CountStones(board, row, col, dx, dy, player, ref openEnds);
            // 역방향
            count += CountStones(board, row, col, -dx, -dy, player, ref openEnds);

            // 점수화
            if (count >= 5) score += 100000;
            else if (count == 4 && openEnds == 2) score += 10000; // 열린 4
            else if (count == 4 && openEnds == 1) score += 1000;  // 막힌 4
            else if (count == 3 && openEnds == 2) score += 100;   // 열린 3
            else if (count == 2 && openEnds == 2) score += 10;    // 열린 2
        }

        return score;
    }

    /// <summary>
    /// 돌 개수 세기 + 열린 끝 체크
    /// </summary>
    private static int CountStones(Constants.PlayerType[,] board, int row, int col,
        int dx, int dy, Constants.PlayerType player, ref int openEnds)
    {
        int size = board.GetLength(0);
        int count = 0;
        int r = row + dx, c = col + dy;

        while (r >= 0 && r < size &&
               c >= 0 && c < size &&
               board[r, c] == player)
        {
            count++;
            r += dx;
            c += dy;
        }

        // 열린 끝(open end) 체크
        if (r >= 0 && r < size &&
            c >= 0 && c < size &&
            board[r, c] == Constants.PlayerType.None)
        {
            openEnds++;
        }

        return count;
    }


    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        int size = board.GetLength(0); // 15
        int winCount = 5;              // 오목 = 5개 연속

        // 4가지 방향 (가로, 세로, 대각선 ↘, 대각선 ↙)
        int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                if (board[row, col] != playerType) continue;

                for (int d = 0; d < directions.GetLength(0); d++)
                {
                    int count = 1;
                    int dx = directions[d, 0];
                    int dy = directions[d, 1];

                    // 앞으로 4칸 더 확인
                    for (int step = 1; step < winCount; step++)
                    {
                        int newRow = row + dx * step;
                        int newCol = col + dy * step;

                        if (newRow < 0 || newRow >= size || newCol < 0 || newCol >= size)
                            break;

                        if (board[newRow, newCol] == playerType)
                            count++;
                        else
                            break;
                    }

                    if (count >= winCount)
                        return true;
                }
            }
        }
        return false;
    }
}
