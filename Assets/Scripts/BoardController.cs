using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable]
public class Coords
{
    public int x; //горизонтальне
    public int y; //вертикальне

    public Coords(int _x = 0, int _y = 0)
    {
        x = _x;
        y = _y;
    }
}

public class BoardController : MonoBehaviour
{
    public int[,] cells; //0 - пустий, 1 - повністю заповнений, >2 - частково заповнений (корабель поруч)

    private GameObject[,] dots;
    public GameObject dotPrefab;

    private Collider2D shipsHolder;

    public GameObject activeGO;
    public GameObject rotationB;

    private int shipsOnBoard = 0;


    void Start()
    {
        shipsHolder = gameObject.GetComponent<Collider2D>();

        cells = new int[GlobalData.BOARD_SIZE, GlobalData.BOARD_SIZE];
        for (int i = 0; i < GlobalData.BOARD_SIZE; i++)
            for (int j = 0; j < GlobalData.BOARD_SIZE; j++)
                cells[i, j] = 0;


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

    }

    public Coords WorldToBoardCoords(Vector3 world)
    {
        return new Coords(Convert.ToInt32(world.x), Convert.ToInt32(world.y));
    }

    public int Check(Ship ship, Coords pos)//чи можна поставити даний корабель на дану позицію
    {
        int RETURN = 0;
        int OK = 1;
        int REMOVE = 2;

        //0 - не можна, треба повернути назад
        //1 - все ок, на дошці
        //2 - все ок, знімаємо з дошки

        Debug.Log(pos.x + " " + pos.y);

        if (ship.isVertical)
        {
            if ((pos.y + ship.size - 1 >= GlobalData.BOARD_SIZE || pos.y < 0) ||
                (pos.x >= GlobalData.BOARD_SIZE || pos.x < 0)) //чекаю чи знаходиться в межах дошки
                return (shipsHolder.OverlapPoint(new Vector2(pos.x, pos.y)) ? REMOVE : RETURN);
            for (int i = 0; i < ship.size; i++)
                if (cells[pos.y + i, pos.x] != 0)
                    return RETURN;
        }
        else //for horizontal
        {
            if ((pos.x + ship.size - 1 >= GlobalData.BOARD_SIZE || pos.x < 0) ||
                (pos.y >= GlobalData.BOARD_SIZE || pos.y < 0))//чекаю чи знаходиться в межах дошки
                return (shipsHolder.OverlapPoint(new Vector2(pos.x, pos.y)) ? REMOVE : RETURN);
            for (int i = 0; i < ship.size; i++)
                if (cells[pos.y, pos.x + i] != 0)
                    return RETURN;
        }

        return OK;
    }

    private void UpdateColors()
    {
        for (int i = 0; i < GlobalData.BOARD_SIZE; i++)
            for (int j = 0; j < GlobalData.BOARD_SIZE; j++)
            {
                if (cells[i, j] == 0)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.cyan;
                if (cells[i, j] == 1)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.red;
                if (cells[i, j] == 2)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.yellow;
                if (cells[i, j] == 4)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                if (cells[i, j] == 6)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.grey;
                if (cells[i, j] >= 8)
                    dots[i, j].GetComponent<SpriteRenderer>().color = Color.magenta;
            }
    }

    public void HelloShip(Ship ship, Coords pos)//коли корабель кладуть на дошку
    {
        Debug.Log("Hello" + Convert.ToString(pos.x) + " " + Convert.ToString(pos.y));
        if (Check(ship, pos) == 1)
        {
            shipsOnBoard++;
            if (ship.isVertical)
            {
                for (int i = 0; i < ship.size + 2; i++) //помічаємо навколо корабля зону 
                    for (int j = 0; j < 3; j++)
                        if ((pos.y + i - 1) >= 0 && (pos.y + i - 1) < GlobalData.BOARD_SIZE &&
                            (pos.x + j - 1) >= 0 && (pos.x + j - 1) < GlobalData.BOARD_SIZE)
                        {
                            cells[pos.y + i - 1, pos.x + j - 1] += 2;
                        }

                for (int i = 0; i < ship.size; i++)
                    cells[pos.y + i, pos.x] = 1;
            }
            else //для горизонтального
            {
                for (int i = 0; i < 3; i++) //помічаємо навколо корабля зону 
                    for (int j = 0; j < ship.size + 2; j++)
                        if ((pos.y + i - 1) >= 0 && (pos.y + i - 1) < GlobalData.BOARD_SIZE &&
                            (pos.x + j - 1) >= 0 && (pos.x + j - 1) < GlobalData.BOARD_SIZE)
                            cells[pos.y + i - 1, pos.x + j - 1] += 2;

                for (int i = 0; i < ship.size; i++)
                    cells[pos.y, pos.x + i] = 1;
            }
        }

        UpdateColors();
    }

    public void ByeShip(Ship ship, Coords pos)
    {
        shipsOnBoard--;
        if (ship.isVertical)
        {
            for (int i = 0; i < ship.size + 2; i++) //помічаємо навколо корабля зону 
                for (int j = 0; j < 3; j++)
                    if ((pos.y + i - 1) >= 0 && (pos.y + i - 1) < GlobalData.BOARD_SIZE &&
                        (pos.x + j - 1) >= 0 && (pos.x + j - 1) < GlobalData.BOARD_SIZE)
                        cells[pos.y + i - 1, pos.x + j - 1] -= 2;

            for (int i = 0; i < ship.size; i++)
                cells[pos.y + i, pos.x] = 0;
        }
        else //для горизонтального
        {
            for (int i = 0; i < 3; i++) //помічаємо навколо корабля зону 
                for (int j = 0; j < ship.size + 2; j++)
                    if ((pos.y + i - 1) >= 0 && (pos.y + i - 1) < GlobalData.BOARD_SIZE &&
                        (pos.x + j - 1) >= 0 && (pos.x + j - 1) < GlobalData.BOARD_SIZE)
                        cells[pos.y + i - 1, pos.x + j - 1] -= 2;

            for (int i = 0; i < ship.size; i++)
                cells[pos.y, pos.x + i] = 0;
        }

        UpdateColors();
    }

    public void ChangeActiveShip(GameObject go)
    {
        activeGO = go;
        Debug.Log(go);
    }

    public void RotateActiveShip()
    {
        //якщо корабель не на дошці. то повертаю його, інакше роблю те саме, але знімаю його перед цим з дошки
        Ship activeShip = activeGO.GetComponent<ShipController>().ship;

        if(Check(activeShip, WorldToBoardCoords(activeGO.transform.position)) != 2)
        {
            ByeShip(activeShip, WorldToBoardCoords(activeGO.transform.position));
            activeGO.transform.position = new Vector3(activeShip.coords.x, activeShip.coords.y);
        }
        
        activeGO.transform.Rotate((activeShip.isVertical ? new Vector3(0, 0, -90) : new Vector3(0, 0, 90)));
        activeShip.isVertical = !activeShip.isVertical;
    }

    public void ConfirmButton()
    {
        Debug.Log(shipsOnBoard + "ships on board");
        if (shipsOnBoard != GlobalData.totalNumberOfShips)
            return;

        GlobalData.CellsState[,] buf = new GlobalData.CellsState[GlobalData.BOARD_SIZE, GlobalData.BOARD_SIZE];
        for (int i = 0; i < GlobalData.BOARD_SIZE; i++)
            for (int j = 0; j < GlobalData.BOARD_SIZE; j++)
                buf[i, j] = (GlobalData.CellsState)(cells[i, j] == 1 ? 1 : 0);

        if (GlobalData.readyFields == 0)
            GlobalData.map_p1 = buf;
        if (GlobalData.readyFields == 1)
            GlobalData.map_p2 = buf;
        GlobalData.readyFields++;

        if (GlobalData.readyFields == 1)
            SceneManager.LoadScene("Arrangement");
        if (GlobalData.readyFields == 2)
            SceneManager.LoadScene("Battle");
    }
}
