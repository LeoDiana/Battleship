using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    public enum CellsState
    {
        empty = 0,
        ship = 1,
        damaged = 2,
        miss = 3,
        killed = 4
    }

    public static CellsState[,] map_p1;
    public static CellsState[,] map_p2;

    public static int readyFields = 0;

    public static bool isFirstPlayerActive = true;

    public static int BOARD_SIZE { get { return 10; } }
    public static int totalNumberOfShips { get { return 10; } }
}
