using System.Text;
using UnityEngine;

public static class RenjuRule
{
    public static bool IsForbiddenMove(Constants.PlayerType[,] board, int row, int col)
    {
        if (board[row, col] != Constants.PlayerType.None) return false;

        board[row, col] = Constants.PlayerType.Player_Black; // Èæµ¹ °¡Á¤
        int openThreeCount = 0;
        int openFourCount = 0;
        bool overline = false;

        (int dx, int dy)[] dirs = { (1, 0), (0, 1), (1, 1), (1, -1) };

        foreach (var (dx, dy) in dirs)
        {
            string line = GetLine(board, row, col, dx, dy);

            if (line.Contains("BBBBBB")) overline = true;
            if (line.Contains(".BBBB.")) openFourCount++;
            if (line.Contains(".BBB.")) openThreeCount++;
        }

        board[row, col] = Constants.PlayerType.None; // º¹¿ø

        if (overline) return true;
        if (openThreeCount >= 2) return true; // »ï»ï
        if (openFourCount >= 2) return true;  // »ç»ç

        return false;
    }

    private static string GetLine(Constants.PlayerType[,] board, int x, int y, int dx, int dy)
    {
        int size = board.GetLength(0);
        StringBuilder sb = new StringBuilder();

        for (int i = 5; i > 0; i--)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            if (nx < 0 || ny < 0 || nx >= size || ny >= size) sb.Append('X');
            else sb.Append(Convert(board[nx, ny]));
        }

        sb.Append(Convert(board[x, y]));

        for (int i = 1; i <= 5; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            if (nx < 0 || ny < 0 || nx >= size || ny >= size) sb.Append('X');
            else sb.Append(Convert(board[nx, ny]));
        }

        return sb.ToString();
    }

    private static char Convert(Constants.PlayerType type)
    {
        switch (type)
        {
            case Constants.PlayerType.Player_Black: return 'B';
            case Constants.PlayerType.Player_White: return 'W';
            default: return '.';
        }
    }
}
