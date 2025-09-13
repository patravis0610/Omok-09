using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class GomokuBoard : MonoBehaviour
{
    [Header("Board")]
    [Range(5, 25)] public int size = 14;     // 14x14
    public float cellSize = 1.0f;            // 격자 간격 (유닛)
    public float stoneScale = 0.9f;          // 돌 크기(셀 대비)
    public Color gridColor = new Color(0f, 0f, 0f, 0.6f);

    [Header("Prefabs & Parents")]
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    public Transform stonesParent;           // 돌들을 담을 부모(비워두면 자동 생성)

    [Header("Input/Camera")]
    public Camera cam;                       // 비워두면 자동으로 Main Camera 사용
    public bool lockAfterWin = true;         // 승리 후 더 이상 착수 금지

    // 0: empty, 1: black, 2: white
    private int[,] board;
    private bool isBlackTurn = true;
    private bool gameOver = false;

    private readonly List<GameObject> spawnedStones = new List<GameObject>();

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (stonesParent == null)
        {
            var go = new GameObject("Stones");
            stonesParent = go.transform;
        }
        board = new int[size, size];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetBoard();
        if (gameOver && lockAfterWin) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetBoardCellUnderMouse(out int r, out int c))
            {
                TryPlace(r, c);
            }
        }
    }

    bool TryGetBoardCellUnderMouse(out int row, out int col)
    {
        row = col = -1;
        if (cam == null) return false;

        // 보드가 있는 z-평면 위 좌표 구하기(원근/직교 카메라 모두 지원)
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, transform.position.z));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out float t)) return false;

        Vector3 world = ray.GetPoint(t);
        Vector2 bottomLeft = GetBottomLeft();

        // 보드 영역 밖 클릭 방지(선 바깥 여백 0.5칸 허용)
        float xOff = world.x - bottomLeft.x;
        float yOff = world.y - bottomLeft.y;

        if (xOff < -cellSize * 0.5f || yOff < -cellSize * 0.5f) return false;
        if (xOff > (size - 1 + 0.5f) * cellSize || yOff > (size - 1 + 0.5f) * cellSize) return false;

        // 교차점(선 교차지점)에 가장 가까운 칸 인덱스: 반올림 사용
        int cGuess = Mathf.RoundToInt(xOff / cellSize);
        int rGuess = Mathf.RoundToInt(yOff / cellSize);

        // 유효 범위 클램프
        col = Mathf.Clamp(cGuess, 0, size - 1);
        row = Mathf.Clamp(rGuess, 0, size - 1);
        return true;
        // 필요 시, 교차점 근접성 체크(멀리 클릭 무시) 추가 가능
        // var center = GetCellWorld(row, col);
        // if (Vector2.Distance(world, center) > cellSize * 0.45f) return false;
    }

    void TryPlace(int r, int c)
    {
        if (board[r, c] != 0) return; // 이미 착수

        int stone = isBlackTurn ? 1 : 2;
        board[r, c] = stone;

        // 돌 생성
        GameObject prefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
        if (prefab != null)
        {
            Vector3 pos = GetCellWorld(r, c);
            var go = Instantiate(prefab, pos, Quaternion.identity, stonesParent);
            go.transform.localScale = Vector3.one * (stoneScale * cellSize);
            spawnedStones.Add(go);
        }

        // 승리 판정
        if (IsWin(r, c, stone))
        {
            gameOver = true;
            string who = (stone == 1) ? "Black" : "White";
            Debug.Log($"Gomoku: {who} wins! (r={r}, c={c})");
        }
        else
        {
            isBlackTurn = !isBlackTurn;
        }
    }

    public void ResetBoard()
    {
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                board[y, x] = 0;

        foreach (var s in spawnedStones)
            if (s) Destroy(s);
        spawnedStones.Clear();

        isBlackTurn = true;
        gameOver = false;
        Debug.Log("Gomoku: Reset");
    }

    // ===== 좌표/그리드 유틸 =====
    Vector2 GetBottomLeft()
    {
        // transform.position을 보드 "중심"으로 가정
        float half = (size - 1) * 0.5f;
        return new Vector2(
            transform.position.x - half * cellSize,
            transform.position.y - half * cellSize
        );
    }

    Vector3 GetCellWorld(int r, int c)
    {
        Vector2 bl = GetBottomLeft();
        return new Vector3(
            bl.x + c * cellSize,
            bl.y + r * cellSize,
            transform.position.z
        );
    }

    // ===== 승리 판정(5목 이상) =====
    bool IsWin(int r, int c, int stone)
    {
        // 4개 방향(가로, 세로, 대각 ↑, 대각 ↓)
        return CountLine(r, c, 1, 0, stone) >= 5   // 가로
            || CountLine(r, c, 0, 1, stone) >= 5   // 세로
            || CountLine(r, c, 1, 1, stone) >= 5   // 대각 ↗
            || CountLine(r, c, 1, -1, stone) >= 5; // 대각 ↘
    }

    int CountLine(int r, int c, int dr, int dc, int stone)
    {
        int count = 1;

        // 정방향
        int rr = r + dr, cc = c + dc;
        while (InBoard(rr, cc) && board[rr, cc] == stone)
        {
            count++; rr += dr; cc += dc;
        }

        // 역방향
        rr = r - dr; cc = c - dc;
        while (InBoard(rr, cc) && board[rr, cc] == stone)
        {
            count++; rr -= dr; cc -= dc;
        }

        return count;
    }

    bool InBoard(int r, int c) => (r >= 0 && r < size && c >= 0 && c < size);

    // ===== 에디터/런타임 그리드 시각화 =====
    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        int s = Mathf.Max(2, size);
        float cs = Mathf.Max(0.1f, cellSize);
        Vector2 bl = GetBottomLeft();

        // 격자 교차선
        for (int i = 0; i < s; i++)
        {
            // 가로
            Vector3 a = new Vector3(bl.x, bl.y + i * cs, transform.position.z);
            Vector3 b = new Vector3(bl.x + (s - 1) * cs, bl.y + i * cs, transform.position.z);
            Gizmos.DrawLine(a, b);

            // 세로
            Vector3 c0 = new Vector3(bl.x + i * cs, bl.y, transform.position.z);
            Vector3 d = new Vector3(bl.x + i * cs, bl.y + (s - 1) * cs, transform.position.z);
            Gizmos.DrawLine(c0, d);
        }
    }
}

