using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBoard : MonoBehaviour
{
    public GlobalData.CellsState[,] cells;

    private GameObject[,] dots;
    public GameObject dotPrefab;
    public Text text;
    public BattleController BC;

    public int forPlayer; 
    public bool isUnderAtack;

    private int killedShips = 0;

    public Sprite cross;
    public GameObject [] ships;

    void Start()
    {
        dots = new GameObject[GlobalData.BOARD_SIZE, GlobalData.BOARD_SIZE];
        for (int i = 0; i < GlobalData.BOARD_SIZE; i++)
            for (int j = 0; j < GlobalData.BOARD_SIZE; j++)
            {
                GameObject go = Instantiate(dotPrefab,
                                          new Vector3(gameObject.transform.position.x + j,
                                                      gameObject.transform.position.y + i),
                                          gameObject.transform.rotation,
                                          gameObject.transform);
                go.name += Convert.ToString(j) + "_" + Convert.ToString(i);
                dots[i, j] = go;
            }
        
        if (forPlayer == 1)
            cells = GlobalData.map_p1;
        if (forPlayer == 2)
            cells = GlobalData.map_p2;
        
        UpdateColors();      
    }

    public void UpdateColors()
    {
        for (int i = 0; i < GlobalData.BOARD_SIZE; i++)
            for (int j = 0; j < GlobalData.BOARD_SIZE; j++)
            {
                if (cells[i, j] == GlobalData.CellsState.empty)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.clear;
                if (cells[i, j] == GlobalData.CellsState.ship)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.clear;
                if (cells[i, j] == GlobalData.CellsState.damaged)
                {
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.yellow;
                    dots[i, j].GetComponent<SpriteRenderer>().sprite = cross;
                    dots[i, j].transform.localScale = new Vector3(0.9f, 0.9f);
                }
                    
                if (cells[i, j] == GlobalData.CellsState.miss)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.cyan;
                if (cells[i, j] == GlobalData.CellsState.killed)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.clear;
            }
    }

    public Coords WorldToBoardCoords(Vector3 world)
    {
        return new Coords(Convert.ToInt32(world.x - gameObject.transform.position.x),
                          Convert.ToInt32(world.y - gameObject.transform.position.y));
    }

    void OnMouseUp()
    {
        if (!isUnderAtack)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Coords c = WorldToBoardCoords(mousePosition);

        if (!(cells[c.y, c.x] == GlobalData.CellsState.empty || cells[c.y, c.x] == GlobalData.CellsState.ship))
            return;
        if (cells[c.y, c.x] == GlobalData.CellsState.empty)
            cells[c.y, c.x] = GlobalData.CellsState.miss;
        if (cells[c.y, c.x] == GlobalData.CellsState.ship)
        {
            cells[c.y, c.x] = GlobalData.CellsState.damaged;
            DFS(c.x, c.y);
            UpdateColors();
            return;
        }  

        UpdateColors();
        BC.EndTurn(forPlayer);
    }

    void PlaceShip(int size, Coords coords)
    {
        if(coords.x + 1 < GlobalData.BOARD_SIZE && cells[coords.y, coords.x + 1] == GlobalData.CellsState.killed)
            Instantiate(ships[size - 1],
                        new Vector3(transform.position.x + coords.x, transform.position.y + coords.y),
                        new Quaternion(0, 0, -0.7f, 0.7f));            
        else
            Instantiate(ships[size - 1], new Vector3(transform.position.x + coords.x, transform.position.y + coords.y), transform.rotation);
    }

    void DFS(int x, int y)
    {
        Coords coords = new Coords(x, y);
        int d = 0;
        List<(int first, int second)> visited = new List<(int first, int second)>();
        if (DFS_check(x, y, ref visited, ref coords, ref d) == 0)//якщо цілих палуб 0, то корабель - вбито
        {
            for (int i = 0; i < visited.Count; i++)
                cells[visited[i].second, visited[i].first] = GlobalData.CellsState.killed;
            killedShips++;
            PlaceShip(d, coords);
            if (killedShips == GlobalData.totalNumberOfShips)
                BC.Win(forPlayer);
        }
    }

    //coords потрібен для того, що б поставити на це місце корабель
    //d - кількість ранених палуб
    int DFS_check(int x, int y, ref List<(int first, int second)> visited, ref Coords coords, ref int d)
    {
        for (int i = 0; i < visited.Count; i++)
            if (visited[i].first == x && visited[i].second == y)
                return 0;
        int k = 0;

        if (cells[y, x] == GlobalData.CellsState.ship)
            k++;
        if (cells[y, x] == GlobalData.CellsState.damaged)
            d++;

        (int first, int second) v = (x, y);
        visited.Add(v);


        if (x + 1 < GlobalData.BOARD_SIZE && (cells[y, x + 1] == GlobalData.CellsState.ship ||
                                              cells[y, x + 1] == GlobalData.CellsState.damaged))
            k += DFS_check(x + 1, y, ref visited, ref coords, ref d);
            
        if (x - 1 >= 0 && (cells[y, x - 1] == GlobalData.CellsState.ship || cells[y, x - 1] == GlobalData.CellsState.damaged))
        {
            coords.x = x - 1;
            k += DFS_check(x - 1, y, ref visited, ref coords, ref d);
        }            
        if (y + 1 < GlobalData.BOARD_SIZE && (cells[y + 1, x] == GlobalData.CellsState.ship ||
                                              cells[y + 1, x] == GlobalData.CellsState.damaged))
            k += DFS_check(x, y + 1, ref visited, ref coords, ref d);
        if (y - 1 >= 0 && (cells[y - 1, x] == GlobalData.CellsState.ship || cells[y - 1, x] == GlobalData.CellsState.damaged))
        {
            coords.y = y - 1;
            k += DFS_check(x, y - 1, ref visited, ref coords, ref d);
        }
            

        return k;
    }
}